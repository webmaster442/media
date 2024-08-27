using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Xml;

using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Server.Properties;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server;

internal sealed partial class MediaMount
  : Logging, IMediaServer, IPrefixHandler
{
    private static uint s_mount;

    private readonly Dictionary<IPAddress, Guid> _guidsForAddresses = new();

    private readonly IMediaServer _server;

    private uint _systemID = 1;

    public MediaMount(IMediaServer aServer)
    {
        _server = aServer;
        Prefix = $"/mm-{++s_mount}/";
        if (_server is IVolatileMediaServer vms)
        {
            vms.Changed += ChangedServer;
        }
    }

    public string DescriptorURI => $"{Prefix}description.xml";

    public IHttpAuthorizationMethod Authorizer => _server.Authorizer;

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
              request.RemoteEndpoint,
              IP.GetMAC(request.RemoteEndpoint.Address)
              ))
        {
            throw new HttpStatusException(HttpCode.Denied);
        }

        var path = request.Path.Substring(Prefix.Length);
        Debug(path);
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
              "contentdirectory"
              );
        }
        if (path == "connectionManager.xml")
        {
            return new ResourceResponse(
              HttpCode.Ok,
              "text/xml",
              "connectionmanager"
              );
        }
        if (path == "MSMediaReceiverRegistrar.xml")
        {
            return new ResourceResponse(
              HttpCode.Ok,
              "text/xml",
              "MSMediaReceiverRegistrar"
              );
        }
        if (path == "control")
        {
            return ProcessSoapRequest(request);
        }
        if (path.StartsWith("file/", StringComparison.Ordinal))
        {
            var id = path.Split('/')[1];
            InfoFormat("Serving file {0}", id);
            var item = GetItem(id) as IMediaResource ?? throw new HttpStatusException(HttpCode.NotFound);
            return new ItemResponse(Prefix, request, item);
        }
        if (path.StartsWith("cover/", StringComparison.Ordinal))
        {
            var id = path.Split('/')[1];
            InfoFormat("Serving cover {0}", id);
            var item = GetItem(id) as IMediaCover ?? throw new HttpStatusException(HttpCode.NotFound);
            return new ItemResponse(Prefix, request, item.Cover, "Interactive");
        }
        if (path.StartsWith("subtitle/", StringComparison.Ordinal))
        {
            var id = path.Split('/')[1];
            InfoFormat("Serving subtitle {0}", id);
            if (GetItem(id) is not IMetaVideoItem item)
            {
                throw new HttpStatusException(HttpCode.NotFound);
            }
            return new ItemResponse(Prefix, request, item.Subtitle, "Background");
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
        WarnFormat("Did not understand {0} {1}", request.Method, path);
        throw new HttpStatusException(HttpCode.NotFound);
    }

    private void ChangedServer(object? sender, EventArgs e)
    {
        SoapCache.Clear();
        InfoFormat("Rescanned mount {0}", UUID);
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
