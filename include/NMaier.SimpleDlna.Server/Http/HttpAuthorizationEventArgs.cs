using System;
using System.Net;

namespace NMaier.SimpleDlna.Server.Http;

public sealed class HttpAuthorizationEventArgs : EventArgs
{
    internal HttpAuthorizationEventArgs(IHeaders headers,
      IPEndPoint remoteEndpoint)
    {
        Headers = headers;
        RemoteEndpoint = remoteEndpoint;
    }

    public bool Cancel { get; set; }

    public IHeaders Headers { get; }

    public IPEndPoint RemoteEndpoint { get; }
}
