using System.Net;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Http;

public sealed class HttpAuthorizer
: Logging, IHttpAuthorizationMethod, IDisposable
{
    private readonly List<IHttpAuthorizationMethod> _methods =
      new();

    private readonly HttpServer server;

    public HttpAuthorizer(HttpServer server, ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(server);
        this.server = server;
        server.OnAuthorizeClient += OnAuthorize;
    }

    public void Dispose()
    {
        if (server != null)
        {
            server.OnAuthorizeClient -= OnAuthorize;
        }
    }

    public bool Authorize(IHeaders headers, IPEndPoint endPoint)
    {
        if (_methods.Count == 0)
        {
            return true;
        }
        try
        {
            return _methods.Any(m => m.Authorize(headers, endPoint));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to authorize");
            return false;
        }
    }

    private void OnAuthorize(object? sender, HttpAuthorizationEventArgs e)
    {
        e.Cancel = !Authorize(e.Headers, e.RemoteEndpoint);
    }

    public void AddMethod(IHttpAuthorizationMethod method)
    {
        ArgumentNullException.ThrowIfNull(method);
        _methods.Add(method);
    }
}