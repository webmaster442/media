using System.Net.Mime;

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
        _dLNASsdpResponder = new DLNASsdpResponder(_webApp.Logger,
                                                   new Guid("4a8f3b30-d62f-40ed-b003-0719db08fdad"),
                                                   _webApp.Port,
                                                   _webApp.GetIpAdresses());

        _webApp.AddEmbeddedFile("/icon-192.png", EmbeddedResources.Icon192Png, MediaTypeNames.Image.Png);
        _webApp.AddEmbeddedFile("/icon-48.png", EmbeddedResources.Icon48Png, MediaTypeNames.Image.Png);
        _webApp.AddEmbeddedFile("/icon-192.jpg", EmbeddedResources.Icon192Jpg, MediaTypeNames.Image.Jpeg);
        _webApp.AddEmbeddedFile("/icon-48.jpg", EmbeddedResources.Icon48Jpg, MediaTypeNames.Image.Jpeg);
        _webApp.AddEmbeddedFile("/rootDesc.xml", EmbeddedResources.RootDescXml, MediaTypeNames.Text.Xml);
        _webApp.AddStreamingRoutes(_mediaDb.All);
    }

    public async Task RunAsync(CancellationToken token)
    {
        await Task.WhenAll(_dLNASsdpResponder.RunAsync(token), _webApp.RunAsync(token));
    }
}
