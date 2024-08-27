using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Responses;

namespace NMaier.SimpleDlna.Server.Handlers;

internal sealed class IconHandler : IPrefixHandler
{
    public string Prefix => "/icon/";

    public IResponse HandleRequest(IRequest req)
    {
        var resource = req.Path.Substring(Prefix.Length);
        var isPNG = resource.EndsWith(
          ".png", StringComparison.OrdinalIgnoreCase);
        return new ResourceResponse(
          HttpCode.Ok,
          isPNG ? "image/png" : "image/jpeg",
          resource
          );
    }
}