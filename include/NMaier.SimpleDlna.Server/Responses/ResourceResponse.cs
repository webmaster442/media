using System.Resources;

using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Properties;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Responses;

internal sealed class ResourceResponse : Logging, IResponse
{
    private readonly byte[] _resource = Array.Empty<byte>();

    public ResourceResponse(HttpCode aStatus, string type, string aResource)
      : this(aStatus, type, Resources.ResourceManager, aResource)
    {
    }

    public ResourceResponse(HttpCode aStatus, string type, ResourceManager aResourceManager, string aResource)
    {
        Status = aStatus;
        try
        {
            if (aResourceManager.GetObject(aResource) is not byte[] obj)
            {
                Error("Resource " + aResource + " not found");
                throw new InvalidOperationException(aResource);
            }
            _resource = obj;

            Headers["Content-Type"] = type;
            Headers["Content-Length"] = _resource?.Length.ToString() ?? "0";
        }
        catch (Exception ex)
        {
            Error("Failed to prepare resource " + aResource, ex);
            throw;
        }
    }

    public Stream Body => new MemoryStream(_resource);

    public IHeaders Headers { get; } = new ResponseHeaders();

    public HttpCode Status { get; }
}