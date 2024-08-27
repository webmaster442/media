using NMaier.SimpleDlna.Server.Http;

namespace NMaier.SimpleDlna.Server.Interfaces;

public interface IMediaServer
{
    IHttpAuthorizationMethod Authorizer { get; }

    string FriendlyName { get; }

    Guid UUID { get; }

    IMediaItem GetItem(string id);
}