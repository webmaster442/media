using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;

using Media.Embedded;

namespace Media.Infrastructure.Dlna;

public class DLNAServer
{
    private readonly WebApp _webApp;

    public DLNAServer(int port)
    {
        _webApp = new WebApp(port);
        _webApp.AddEmbeddedFile("/icon-192.png", EmbeddedResources.Icon192Png, MediaTypeNames.Image.Png);
        _webApp.AddEmbeddedFile("/icon-48.png", EmbeddedResources.Icon48Png, MediaTypeNames.Image.Png);
        _webApp.AddEmbeddedFile("/icon-192.jpg", EmbeddedResources.Icon192Jpg, MediaTypeNames.Image.Jpeg);
        _webApp.AddEmbeddedFile("/icon-48.jpg", EmbeddedResources.Icon48Jpg, MediaTypeNames.Image.Jpeg);
        _webApp.AddEmbeddedFile("/rootDesc.xml", EmbeddedResources.RootDescXml, MediaTypeNames.Text.Xml);
    }
}
