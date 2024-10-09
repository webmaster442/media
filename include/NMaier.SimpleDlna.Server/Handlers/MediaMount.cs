using System.Net;
using System.Reflection;
using System.Xml;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Interfaces.Metadata;
using NMaier.SimpleDlna.Server.Properties;
using NMaier.SimpleDlna.Server.Responses;
using NMaier.SimpleDlna.Server.Types;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server;

internal sealed partial class MediaMount
  : Logging, IMediaServer, IPrefixHandler
{
    private static uint s_mount;

    private readonly Dictionary<IPAddress, Guid> _guidsForAddresses = new();

    private readonly IMediaServer _server;

    private uint _systemID = 1;

    public MediaMount(IMediaServer aServer, ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        _server = aServer;
        Prefix = $"/mm-{++s_mount}/";
        if (_server is IVolatileMediaServer vms)
        {
            vms.Changed += ChangedServer;
        }
    }

    public string DescriptorURI => $"{Prefix}description.xml";

    public IHttpAuthorizationMethod? Authorizer => _server.Authorizer;

    public string FriendlyName => _server.FriendlyName;

    public Guid UUID => _server.UUID;

    public IMediaItem GetItem(string id)
    {
        return _server.GetItem(id);
    }

    public string Prefix { get; }

    public IResponse HandleRequest(IRequest request)
    {
        if (Authorizer != null &&
            !IPAddress.IsLoopback(request.RemoteEndpoint.Address) &&
            !Authorizer.Authorize(
              request.Headers,
              request.RemoteEndpoint))
        {
            throw new HttpStatusException(HttpCode.Denied);
        }

        var path = request.Path.Substring(Prefix.Length);
        Logger.LogDebug(path);
        if (path == "description.xml")
        {
            return new StringResponse(
              HttpCode.Ok,
              "text/xml",
              GenerateDescriptor(request.LocalEndPoint.Address)
              );
        }
        if (path == "contentDirectory.xml")
        {
            return new ResourceResponse(
              HttpCode.Ok,
              "text/xml",
              "contentdirectory",
              LoggerFactory);
        }
        if (path == "connectionManager.xml")
        {
            return new ResourceResponse(
              HttpCode.Ok,
              "text/xml",
              "connectionmanager",
              LoggerFactory);
        }
        if (path == "MSMediaReceiverRegistrar.xml")
        {
            return new ResourceResponse(
              HttpCode.Ok,
              "text/xml",
              "MSMediaReceiverRegistrar",
              LoggerFactory);
        }
        if (path == "control")
        {
            return ProcessSoapRequest(request);
        }
        if (path.StartsWith("file/", StringComparison.Ordinal))
        {
            var id = path.Split('/')[1];
            Logger.LogInformation("Serving file {id}", id);
            var item = GetItem(id) as IMediaResource ?? throw new HttpStatusException(HttpCode.NotFound);
            return new ItemResponse(Prefix, request, item, LoggerFactory);
        }
        if (path.StartsWith("cover/", StringComparison.Ordinal))
        {
            var id = path.Split('/')[1];
            Logger.LogInformation("Serving cover {id}", id);
            var item = GetItem(id) as IMediaCover ?? throw new HttpStatusException(HttpCode.NotFound);
            return new ItemResponse(Prefix, request, item.Cover, LoggerFactory, "Interactive");
        }
        if (path.StartsWith("subtitle/", StringComparison.Ordinal))
        {
            var id = path.Split('/')[1];
            Logger.LogInformation("Serving subtitle {id}", id);
            if (GetItem(id) is not IMetaVideoItem item)
            {
                throw new HttpStatusException(HttpCode.NotFound);
            }
            return new ItemResponse(Prefix, request, item.Subtitle, LoggerFactory, "Background");
        }

        if (string.IsNullOrEmpty(path) || path == "index.html")
        {
            return new Redirect(request, Prefix + "index/0");
        }
        if (path.StartsWith("index/", StringComparison.Ordinal))
        {
            var id = path.Substring("index/".Length);
            var item = GetItem(id);
            return ProcessHtmlRequest(item);
        }
        if (request.Method == "SUBSCRIBE")
        {
            var res = new StringResponse(HttpCode.Ok, string.Empty);
            res.Headers.Add("SID", $"uuid:{Guid.NewGuid()}");
            res.Headers.Add("TIMEOUT", request.Headers["timeout"]);
            return res;
        }
        if (request.Method == "UNSUBSCRIBE")
        {
            return new StringResponse(HttpCode.Ok, string.Empty);
        }
        Logger.LogWarning("Did not understand {method} {path}", request.Method, path);
        throw new HttpStatusException(HttpCode.NotFound);
    }

    private void ChangedServer(object? sender, EventArgs e)
    {
        SoapCache.Clear();
        Logger.LogInformation("Rescanned mount {uuid}", UUID);
        _systemID++;
    }

    private XmlNode SelectSingleNode(XmlDocument document, string xPath)
    {
        var result = document.SelectSingleNode(xPath);
        return result ?? throw new InvalidOperationException($"select failed: {xPath}");
    }

    private string GenerateDescriptor(IPAddress source)
    {
        var doc = new XmlDocument();
        doc.LoadXml(Resources.description);
        Guid guid;
        _guidsForAddresses.TryGetValue(source, out guid);
        SelectSingleNode(doc, "//*[local-name() = 'UDN']")
            .InnerText = $"uuid:{guid}";
        SelectSingleNode(doc, "//*[local-name() = 'modelNumber']")
            .InnerText = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
        SelectSingleNode(doc, "//*[local-name() = 'friendlyName']")
            .InnerText = FriendlyName + " — sdlna";

        SelectSingleNode(doc, "//*[text() = 'urn:schemas-upnp-org:service:ContentDirectory:1']/../*[local-name() = 'SCPDURL']")
            .InnerText = $"{Prefix}contentDirectory.xml";
        SelectSingleNode(doc, "//*[text() = 'urn:schemas-upnp-org:service:ContentDirectory:1']/../*[local-name() = 'controlURL']")
            .InnerText = $"{Prefix}control";

        SelectSingleNode(doc, "//*[local-name() = 'eventSubURL']")
            .InnerText = $"{Prefix}events";

        SelectSingleNode(doc, "//*[text() = 'urn:schemas-upnp-org:service:ConnectionManager:1']/../*[local-name() = 'SCPDURL']")
            .InnerText = $"{Prefix}connectionManager.xml";
        SelectSingleNode(doc, "//*[text() = 'urn:schemas-upnp-org:service:ConnectionManager:1']/../*[local-name() = 'controlURL']")
            .InnerText = $"{Prefix}control";

        SelectSingleNode(doc, "//*[text() = 'urn:schemas-upnp-org:service:ConnectionManager:1']/../*[local-name() = 'eventSubURL']")
            .InnerText = $"{Prefix}events";

        SelectSingleNode(doc, "//*[text() = 'urn:schemas-upnp-org:service:X_MS_MediaReceiverRegistrar:1']/../*[local-name() = 'SCPDURL']")
            .InnerText = $"{Prefix}MSMediaReceiverRegistrar.xml";

        SelectSingleNode(doc, "//*[text() = 'urn:schemas-upnp-org:service:X_MS_MediaReceiverRegistrar:1']/../*[local-name() = 'controlURL']")
          .InnerText = $"{Prefix}control";

        SelectSingleNode(doc, "//*[text() = 'urn:schemas-upnp-org:service:X_MS_MediaReceiverRegistrar:1']/../*[local-name() = 'eventSubURL']")
          .InnerText = $"{Prefix}events";

        return doc.OuterXml;
    }

    public void AddDeviceGuid(Guid guid, IPAddress address)
    {
        _guidsForAddresses.Add(address, guid);
    }
}