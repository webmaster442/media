using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Server.Types;
using NMaier.SimpleDlna.Server.Utilities;

using Timer = System.Timers.Timer;

namespace NMaier.SimpleDlna.Server.Ssdp;

internal sealed class SsdpHandler : Logging, IDisposable
{
    private const int DATAGRAMS_PER_MESSAGE = 3;

    private const string SSDP_ADDR = "239.255.255.250";

    private const int SSDP_PORT = 1900;

    private static readonly IPEndPoint SsdpEndp =
      new IPEndPoint(IPAddress.Parse(SSDP_ADDR), SSDP_PORT);

    internal static readonly IPEndPoint BroadEndp =
      new IPEndPoint(IPAddress.Parse("255.255.255.255"), SSDP_PORT);

    private static readonly IPAddress SsdpIP =
      IPAddress.Parse(SSDP_ADDR);

    private readonly UdpClient _client = new UdpClient();

    private readonly AutoResetEvent _datagramPosted =
      new AutoResetEvent(false);

    private readonly Dictionary<Guid, List<UpnpDevice>> _devices =
      new Dictionary<Guid, List<UpnpDevice>>();

    private readonly ConcurrentQueue<Datagram> _messageQueue =
      new ConcurrentQueue<Datagram>();

    private readonly Timer _notificationTimer =
      new Timer(60000);

    private readonly Timer _queueTimer =
      new Timer(1000);

    private bool _running = true;

    public SsdpHandler(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        _notificationTimer.Elapsed += Tick;
        _notificationTimer.Enabled = true;

        _queueTimer.Elapsed += ProcessQueue;

        _client.Client.SetSocketOption(
          SocketOptionLevel.Socket,
          SocketOptionName.ReuseAddress,
          true
          );
        _client.ExclusiveAddressUse = false;
        _client.Client.Bind(new IPEndPoint(IPAddress.Any, SSDP_PORT));
        _client.JoinMulticastGroup(SsdpIP, 10);
        Logger.LogInformation("SSDP service started");
        Receive();
    }

    private UpnpDevice[] Devices
    {
        get
        {
            UpnpDevice[] devs;
            lock (_devices)
            {
                devs = _devices.Values.SelectMany(i => i).ToArray();
            }
            return devs;
        }
    }

    public void Dispose()
    {
        Logger.LogDebug("Disposing SSDP");
        _running = false;
        while (!_messageQueue.IsEmpty)
        {
            _datagramPosted.WaitOne();
        }

        _client.DropMulticastGroup(SsdpIP);

        _notificationTimer.Enabled = false;
        _queueTimer.Enabled = false;
        _notificationTimer.Dispose();
        _queueTimer.Dispose();
        _client.Dispose();
        _datagramPosted.Dispose();
    }

    private void ProcessQueue(object? sender, ElapsedEventArgs e)
    {
        while (!_messageQueue.IsEmpty)
        {
            if (!_messageQueue.TryPeek(out Datagram? msg))
            {
                continue;
            }
            if (msg != null && (_running || msg.Sticky))
            {
                msg.Send();
                if (msg.SendCount > DATAGRAMS_PER_MESSAGE)
                {
                    _messageQueue.TryDequeue(out msg);
                }
                break;
            }
            _messageQueue.TryDequeue(out msg);
        }
        _datagramPosted.Set();
        _queueTimer.Enabled = !_messageQueue.IsEmpty;
        _queueTimer.Interval = Random.Shared.Next(25, _running ? 75 : 50);
    }

    private void Receive()
    {
        try
        {
            _client.BeginReceive(ReceiveCallback, null);
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            IPEndPoint? endpoint = new IPEndPoint(IPAddress.None, SSDP_PORT);
            var received = _client.EndReceive(result, ref endpoint) ?? throw new IOException("Didn't receive anything");
            if (received.Length == 0)
            {
                throw new IOException("Didn't receive any bytes");
            }
#if DUMP_ALL_SSDP
    DebugFormat("{0} - SSDP Received a datagram", endpoint);
#endif
            using (var reader = new StreamReader(new MemoryStream(received), Encoding.ASCII))
            {
                var proto = reader.ReadLine();
                if (proto == null)
                {
                    throw new IOException("Couldn't read protocol line");
                }
                proto = proto.Trim();
                if (string.IsNullOrEmpty(proto))
                {
                    throw new IOException("Invalid protocol line");
                }
                var method = proto.Split(new[] { ' ' }, 2)[0];
                var headers = new Headers();
                for (var line = reader.ReadLine();
                  line != null;
                  line = reader.ReadLine())
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    var parts = line.Split(new[] { ':' }, 2);
                    headers[parts[0]] = parts[1].Trim();
                }
#if DUMP_ALL_SSDP
      DebugFormat("{0} - Datagram method: {1}", endpoint, method);
      Debug(headers);
#endif
                if (method == "M-SEARCH" && endpoint != null)
                {
                    RespondToSearch(endpoint, headers["st"]);
                }
            }
        }
        catch (IOException ex)
        {
            Logger.LogDebug(ex, "Failed to read SSDP message");
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to read SSDP message");
        }
        Receive();
    }

    private void SendDatagram(IPEndPoint endpoint, IPAddress address,
      string message, bool sticky)
    {
        if (!_running)
        {
            return;
        }
        var dgram = new Datagram(endpoint, address, message, sticky, LoggerFactory);
        if (_messageQueue.IsEmpty)
        {
            dgram.Send();
        }
        _messageQueue.Enqueue(dgram);
        _queueTimer.Enabled = true;
    }

    private void SendSearchResponse(IPEndPoint endpoint, UpnpDevice dev)
    {
        var headers = new RawHeaders
  {
    {"CACHE-CONTROL", "max-age = 600"},
    {"DATE", DateTime.Now.ToString("R")},
    {"EXT", string.Empty},
    {"LOCATION", dev.Descriptor.ToString()},
    {"SERVER", HttpServer.Signature},
    {"ST", dev.Type},
    {"USN", dev.USN}
  };

        SendDatagram(
          endpoint,
          dev.Address,
          $"HTTP/1.1 200 OK\r\n{headers.HeaderBlock}\r\n",
          false
          );
        Logger.LogInformation(
          "{2}, {1} - Responded to a {0} request", dev.Type, endpoint,
          dev.Address);
    }

    private void Tick(object? sender, ElapsedEventArgs e)
    {
        Logger.LogDebug("Sending SSDP notifications!");
        _notificationTimer.Interval = Random.Shared.Next(60000, 120000);
        NotifyAll();
    }

    internal void NotifyAll()
    {
        Logger.LogDebug("NotifyAll");
        foreach (var d in Devices)
        {
            NotifyDevice(d, "alive", false);
        }
    }

    internal void NotifyDevice(UpnpDevice dev, string type, bool sticky)
    {
        Logger.LogDebug("NotifyDevice");
        var headers = new RawHeaders
  {
    {"HOST", "239.255.255.250:1900"},
    {"CACHE-CONTROL", "max-age = 600"},
    {"LOCATION", dev.Descriptor.ToString()},
    {"SERVER", HttpServer.Signature},
    {"NTS", "ssdp:" + type},
    {"NT", dev.Type},
    {"USN", dev.USN}
  };

        SendDatagram(
          SsdpEndp,
          dev.Address,
          $"NOTIFY * HTTP/1.1\r\n{headers.HeaderBlock}\r\n",
          sticky
          );
        // Some buggy network equipment will swallow multicast packets, so lets
        // cheat, increase the odds, by sending to broadcast.
        SendDatagram(
          BroadEndp,
          dev.Address,
          $"NOTIFY * HTTP/1.1\r\n{headers.HeaderBlock}\r\n",
          sticky
          );
        Logger.LogDebug("{usn} said {type}", dev.USN, type);
    }

    internal void RegisterNotification(Guid uuid, Uri descriptor,
      IPAddress address)
    {
        List<UpnpDevice>? list;
        lock (_devices)
        {
            if (!_devices.TryGetValue(uuid, out list))
            {
                _devices.Add(uuid, list = new List<UpnpDevice>());
            }
        }
        list.AddRange(new[]
        {
            "upnp:rootdevice", "urn:schemas-upnp-org:device:MediaServer:1",
            "urn:schemas-upnp-org:service:ContentDirectory:1", "urn:schemas-upnp-org:service:ConnectionManager:1",
            "urn:schemas-upnp-org:service:X_MS_MediaReceiverRegistrar:1", "uuid:" + uuid
        }.Select(t => new UpnpDevice(uuid, t, descriptor, address)));

        NotifyAll();
        Logger.LogDebug("Registered mount {uuid}, {address}", uuid, address);
    }

    internal void RespondToSearch(IPEndPoint endpoint, string? req)
    {
        if (req == "ssdp:all")
        {
            req = null;
        }

        Logger.LogDebug("RespondToSearch {endpoint} {req}", endpoint, req);
        foreach (var d in Devices)
        {
            if (!string.IsNullOrEmpty(req) && req != d.Type)
            {
                continue;
            }
            SendSearchResponse(endpoint, d);
        }
    }

    internal void UnregisterNotification(Guid uuid)
    {
        List<UpnpDevice>? dl;
        lock (_devices)
        {
            if (!_devices.TryGetValue(uuid, out dl))
            {
                return;
            }
            _devices.Remove(uuid);
        }
        foreach (var d in dl)
        {
            NotifyDevice(d, "byebye", true);
        }
        Logger.LogDebug("Unregistered mount {uuid}", uuid);
    }
}