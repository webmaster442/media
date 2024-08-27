using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Http;

public sealed class HttpAuthorizer
: Logging, IHttpAuthorizationMethod, IDisposable
{
    private readonly List<IHttpAuthorizationMethod> _methods =
      new();

    private readonly HttpServer server;

    public HttpAuthorizer(HttpServer server)
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

    public bool Authorize(IHeaders headers, IPEndPoint endPoint, string mac)
    {
        if (_methods.Count == 0)
        {
            return true;
        }
        try
        {
            return _methods.Any(m => m.Authorize(headers, endPoint, mac));
        }
        catch (Exception ex)
        {
            Error("Failed to authorize", ex);
            return false;
        }
    }

    private void OnAuthorize(object? sender, HttpAuthorizationEventArgs e)
    {
        e.Cancel = !Authorize(e.Headers,
                              e.RemoteEndpoint,
                              IP.GetMAC(e.RemoteEndpoint.Address));
    }

    public void AddMethod(IHttpAuthorizationMethod method)
    {
        ArgumentNullException.ThrowIfNull(method);
        _methods.Add(method);
    }
}
