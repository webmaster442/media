using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

using NMaier.SimpleDlna.Server.Http;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

namespace Media.Infrastructure.Dlna;

internal sealed class MediaServer : IMediaServer
{
    private readonly ConcurrentDictionary<string, IMediaItem> _items;
    private readonly VirtualFolder _files;

    public MediaServer()
    {
        _files = new VirtualFolder(null, "Media");
        _files.AddResource(new SimpleFileItem("d:\\video\\The Mind Behind Windows： Dave Cutler.mp4"));
        _items = new ConcurrentDictionary<string, IMediaItem>();
        _items.TryAdd("0", _files);
    }

    public IHttpAuthorizationMethod Authorizer
        => new InternalAuthorizer();

    internal class InternalAuthorizer : IHttpAuthorizationMethod
    {
        public bool Authorize(IHeaders headers, IPEndPoint endPoint, string mac)
            => true;
    }

    public string FriendlyName => "Media Cli DLNA Server";

    public Guid UUID => new Guid("B4D9888B-72FC-4DAD-B2AD-120524D191F9");

    public IMediaItem GetItem(string id)
    {
        return _items.TryGetValue(id, out IMediaItem? item) ? item : _files;
    }
}
