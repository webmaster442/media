using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;

using Media.Embedded;

namespace Media.Infrastructure.Dlna;

public class DLNAServer
{
    private readonly WebApp _webApp;
    private readonly MediaDb _mediaDb;
    private readonly DLNASsdpResponder _dLNASsdpResponder;

    public DLNAServer(int port, string directory)
    {
        _webApp = new WebApp(port);
        _mediaDb = new MediaDb();
        _mediaDb.ScanFolder(directory);
        _dLNASsdpResponder = new DLNASsdpResponder(_webApp.Logger, _webApp.GetListenUrls());

        _webApp.AddEmbeddedFile("/icon-192.png", EmbeddedResources.Icon192Png, MediaTypeNames.Image.Png);
        _webApp.AddEmbeddedFile("/icon-48.png", EmbeddedResources.Icon48Png, MediaTypeNames.Image.Png);
        _webApp.AddEmbeddedFile("/icon-192.jpg", EmbeddedResources.Icon192Jpg, MediaTypeNames.Image.Jpeg);
        _webApp.AddEmbeddedFile("/icon-48.jpg", EmbeddedResources.Icon48Jpg, MediaTypeNames.Image.Jpeg);
        _webApp.AddEmbeddedFile("/rootDesc.xml", EmbeddedResources.RootDescXml, MediaTypeNames.Text.Xml);
        _webApp.AddStreamingRoutes(_mediaDb.All);
    }

    public async Task Run(CancellationToken token)
    {
        await _webApp.RunAsync(token);
        await _dLNASsdpResponder.RunAsync(token);
    }
}
