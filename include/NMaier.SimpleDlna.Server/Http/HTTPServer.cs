using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Timers;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Handlers;
using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Responses;
using NMaier.SimpleDlna.Server.Ssdp;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Http;

public sealed class HttpServer : Logging, IDisposable
{
    public static string Signature = string.Empty;

    private readonly ConcurrentDictionary<HttpClient, DateTime> _clients =
      new();

    private readonly ConcurrentDictionary<Guid, List<Guid>> _devicesForServers =
      new();

    private readonly TcpListener _listener;

    private readonly ConcurrentDictionary<string, IPrefixHandler> _prefixes =
      new();

    private readonly ConcurrentDictionary<Guid, MediaMount> _servers =
      new();

    private readonly SsdpHandler _ssdpServer;

    private readonly System.Timers.Timer _timeouter = new(10 * 1000);

    public HttpServer(ILoggerFactory loggerFactory)
      : this(0, loggerFactory)
    {
    }

    public HttpServer(int port, ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        Signature = GenerateServerSignature();

        _prefixes.TryAdd(
          "/favicon.ico",
          new StaticHandler(
            new ResourceResponse(HttpCode.Ok, "image/icon", "favicon", loggerFactory))
          );
        _prefixes.TryAdd(
          "/static/browse.css",
          new StaticHandler(
            new ResourceResponse(HttpCode.Ok, "text/css", "browse_css", loggerFactory))
          );
        RegisterHandler(new IconHandler(loggerFactory));

        _listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
        _listener.Server.Ttl = 32;
        _listener.Start();

        RealPort = ((IPEndPoint)_listener.LocalEndpoint).Port;

        Logger.LogDebug("Running HTTP Server: {signature} on port {port}", Signature, RealPort);

        _timeouter.Elapsed += TimeouterCallback;
        _timeouter.Enabled = true;

        Accept();
    }

    public Dictionary<string, string> MediaMounts
    {
        get
        {
            var rv = new Dictionary<string, string>();
            foreach (var m in _servers)
            {
                rv[m.Value.Prefix] = m.Value.FriendlyName;
            }
            return rv;
        }
    }

    public int RealPort { get; }

    public void Dispose()
    {
        Logger.LogDebug("Disposing HTTP");
        _timeouter.Enabled = false;
        foreach (var s in _servers.Values.ToList())
        {
            UnregisterMediaServer(s);
        }
        _ssdpServer.Dispose();
        _timeouter.Dispose();
        _listener.Stop();
        foreach (var c in _clients.ToList())
        {
            c.Key.Dispose();
        }
        _clients.Clear();
        _listener.Dispose();
    }

    public event EventHandler<HttpAuthorizationEventArgs>? OnAuthorizeClient;

    private void Accept()
    {
        try
        {
            if (!_listener.Server.IsBound)
            {
                return;
            }
            _listener.BeginAcceptTcpClient(AcceptCallback, null);
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to accept");
        }
    }

    private void AcceptCallback(IAsyncResult result)
    {
        try
        {
            var tcpclient = _listener.EndAcceptTcpClient(result);
            var client = new HttpClient(this, tcpclient, LoggerFactory);
            try
            {
                _clients.AddOrUpdate(client, DateTime.Now, (k, v) => DateTime.Now);
                Logger.LogDebug("Accepted client {client}", client);
                client.Start();
            }
            catch (Exception)
            {
                client.Dispose();
                throw;
            }
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to accept a client");
        }
        finally
        {
            Accept();
        }
    }

    private string GenerateServerSignature()
    {
        var os = Environment.OSVersion;
        var pstring = os.Platform.ToString();
        switch (os.Platform)
        {
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
                pstring = "WIN";
                break;

            default:
                try
                {
                    pstring = Formatting.GetSystemName();
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, "Failed to get uname");
                }
                break;
        }
        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0, 0);
        var bitness = nint.Size * 8;
        return $"{pstring}{bitness}/{os.Version.Major}.{os.Version.Minor} UPnP/1.0 DLNADOC/1.5 sdlna/{version.Major}.{version.Minor}";
    }

    private void TimeouterCallback(object? sender, ElapsedEventArgs e)
    {
        foreach (var c in _clients.ToList())
        {
            if (c.Key.IsATimeout)
            {
                Logger.LogDebug("Collected timeout client: {c}", c);
                c.Key.Close();
            }
        }
    }

    internal bool AuthorizeClient(HttpClient client)
    {
        if (OnAuthorizeClient == null)
        {
            return true;
        }
        if (IPAddress.IsLoopback(client.RemoteEndpoint.Address))
        {
            return true;
        }
        var e = new HttpAuthorizationEventArgs(
          client.Headers, client.RemoteEndpoint);
        OnAuthorizeClient(this, e);
        return !e.Cancel;
    }

    internal IPrefixHandler? FindHandler(string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            throw new ArgumentNullException(nameof(prefix));
        }

        if (prefix == "/")
        {
            return new IndexHandler(this);
        }

        return (from s in _prefixes.Keys
                where prefix.StartsWith(s, StringComparison.Ordinal)
                select _prefixes[s]).FirstOrDefault();
    }

    internal void RegisterHandler(IPrefixHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        var prefix = handler.Prefix;
        if (!prefix.StartsWith('/'))
        {
            throw new ArgumentException("Invalid prefix; must start with /");
        }
        if (!prefix.EndsWith('/'))
        {
            throw new ArgumentException("Invalid prefix; must end with /");
        }
        if (FindHandler(prefix) != null)
        {
            throw new ArgumentException("Invalid prefix; already taken");
        }
        if (!_prefixes.TryAdd(prefix, handler))
        {
            throw new ArgumentException("Invalid preifx; already taken");
        }
        Logger.LogDebug("Registered Handler for {prefix}", prefix);
    }

    internal void RemoveClient(HttpClient client)
    {
        _clients.TryRemove(client, out DateTime ignored);
    }

    internal void UnregisterHandler(IPrefixHandler handler)
    {
        if (_prefixes.TryRemove(handler.Prefix, out IPrefixHandler? ignored))
        {
            Logger.LogDebug("Unregistered Handler for {prefix}", handler.Prefix);
        }
    }

    public void RegisterMediaServer(IMediaServer server)
    {
        ArgumentNullException.ThrowIfNull(server);
        var guid = server.UUID;
        if (_servers.ContainsKey(guid))
        {
            throw new ArgumentException("Attempting to register more than once");
        }

        var end = (IPEndPoint)_listener.LocalEndpoint;
        var mount = new MediaMount(server, LoggerFactory);
        _servers[guid] = mount;
        RegisterHandler(mount);

        foreach (var address in IP.ExternalIPAddresses)
        {
            Logger.LogDebug("Registering device for {adress}", address);
            var deviceGuid = Guid.NewGuid();
            var list = _devicesForServers.GetOrAdd(guid, new List<Guid>());
            lock (list)
            {
                list.Add(deviceGuid);
            }
            mount.AddDeviceGuid(deviceGuid, address);
            var uri = new Uri($"http://{address}:{end.Port}{mount.DescriptorURI}");
            lock (list)
            {
                _ssdpServer.RegisterNotification(deviceGuid, uri, address);
            }

            Logger.LogInformation("New mount at: {uri}", uri);
        }
    }

    public void UnregisterMediaServer(IMediaServer server)
    {
        ArgumentNullException.ThrowIfNull(server);
        if (!_servers.TryGetValue(server.UUID, out MediaMount? mount))
        {
            return;
        }

        if (_devicesForServers.TryGetValue(server.UUID, out List<Guid>? list))
        {
            lock (list)
            {
                foreach (var deviceGuid in list)
                {
                    _ssdpServer.UnregisterNotification(deviceGuid);
                }
            }
            _devicesForServers.TryRemove(server.UUID, out list);
        }

        UnregisterHandler(mount);

        if (_servers.TryRemove(server.UUID, out MediaMount? ignored))
        {
            Logger.LogInformation("Unregistered Media Server {uuid}", server.UUID);
        }
    }
}