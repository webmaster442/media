﻿using System.Text;
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

internal partial class MediaMount
{
    private const string NS_DC = "http://purl.org/dc/elements/1.1/";

    private const string NS_DIDL =
      "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/";

    private const string NS_DLNA =
      "urn:schemas-dlna-org:metadata-1-0/";

    private const string NS_SEC = "http://www.sec.co.kr/";

    private const string NS_SOAPENV =
      "http://schemas.xmlsoap.org/soap/envelope/";

    private const string NS_UPNP = "urn:schemas-upnp-org:metadata-1-0/upnp/";

    private static readonly string featureList = Encoding.UTF8.GetString(Resources.x_featurelist);

    private static readonly IDictionary<string, AttributeCollection> SoapCache =
      new LeastRecentlyUsedDictionary<string, AttributeCollection>(200);

    private static readonly XmlNamespaceManager namespaceMgr =
      CreateNamespaceManager();

    private static void AddBookmarkInfo(IMediaResource resource, XmlElement item)
    {
        var bookmarkable = resource as IBookmarkable;
        var bookmark = bookmarkable?.Bookmark;
        if (bookmark != null)
        {
            var dcmInfo = item.OwnerDocument?.CreateElement(
              "sec", "dcmInfo", NS_SEC);
            if (dcmInfo != null)
            {
                dcmInfo.InnerText = $"BM={bookmark.Value}";
                item.AppendChild(dcmInfo);
            }
        }
    }

    private void AddCover(IRequest request, IMediaItem resource, XmlNode item)
    {
        var result = item.OwnerDocument;
        if (result == null)
        {
            return;
        }
        if (resource is not IMediaCover cover)
        {
            return;
        }
        try
        {
            var c = cover.Cover;
            var curl =
              $"http://{request.LocalEndPoint.Address}:{request.LocalEndPoint.Port}{Prefix}cover/{resource.Id}/i.jpg";
            var icon = result.CreateElement("upnp", "albumArtURI", NS_UPNP);
            var profile = result.CreateAttribute("dlna", "profileID", NS_DLNA);
            profile.InnerText = "JPEG_TN";
            icon.SetAttributeNode(profile);
            icon.InnerText = curl;
            item.AppendChild(icon);
            icon = result.CreateElement("upnp", "icon", NS_UPNP);
            profile = result.CreateAttribute("dlna", "profileID", NS_DLNA);
            profile.InnerText = "JPEG_TN";
            icon.SetAttributeNode(profile);
            icon.InnerText = curl;
            item.AppendChild(icon);

            var res = result.CreateElement(string.Empty, "res", NS_DIDL);
            res.InnerText = curl;

            res.SetAttribute("protocolInfo", string.Format(
              "http-get:*:{1}:DLNA.ORG_PN={0};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS={2}",
              c.PN, DlnaMaps.Mime[c.Type], DlnaMaps.DefaultStreaming
                                               ));
            var width = c.MetaWidth;
            var height = c.MetaHeight;
            if (width.HasValue && height.HasValue)
            {
                res.SetAttribute("resolution", $"{width.Value}x{height.Value}");
            }
            else
            {
                res.SetAttribute("resolution", "200x200");
            }
            res.SetAttribute("protocolInfo",
                             $"http-get:*:image/jpeg:DLNA.ORG_PN=JPEG_TN;DLNA.ORG_OP=01;DLNA.ORG_CI=1;DLNA.ORG_FLAGS={DlnaMaps.DefaultInteractive}");
            item.AppendChild(res);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Addcover failed");
            // ignored
        }
    }

    private static void AddGeneralProperties(IHeaders props, XmlElement item)
    {
        string? prop;
        var ownerDocument = item.OwnerDocument;
        if (ownerDocument == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (props.TryGetValue("DateO", out prop))
        {
            var e = ownerDocument.CreateElement("dc", "date", NS_DC);
            e.InnerText = prop;
            item.AppendChild(e);
        }
        if (props.TryGetValue("Genre", out prop))
        {
            var e = ownerDocument.CreateElement("upnp", "genre", NS_UPNP);
            e.InnerText = prop;
            item.AppendChild(e);
        }
        if (props.TryGetValue("Description", out prop))
        {
            var e = ownerDocument.CreateElement("dc", "description", NS_DC);
            e.InnerText = prop;
            item.AppendChild(e);
        }
        if (props.TryGetValue("Artist", out prop))
        {
            var e = ownerDocument.CreateElement("upnp", "artist", NS_UPNP);
            e.SetAttribute("role", "AlbumArtist");
            e.InnerText = prop;
            item.AppendChild(e);
        }
        if (props.TryGetValue("Performer", out prop))
        {
            var e = ownerDocument.CreateElement("upnp", "artist", NS_UPNP);
            e.SetAttribute("role", "Performer");
            e.InnerText = prop;
            item.AppendChild(e);
            e = ownerDocument.CreateElement("dc", "creator", NS_DC);
            e.InnerText = prop;
            item.AppendChild(e);
        }
        if (props.TryGetValue("Album", out prop))
        {
            var e = ownerDocument.CreateElement("upnp", "album", NS_UPNP);
            e.InnerText = prop;
            item.AppendChild(e);
        }
        if (props.TryGetValue("Track", out prop))
        {
            var e = ownerDocument.CreateElement(
              "upnp", "originalTrackNumber", NS_UPNP);
            e.InnerText = prop;
            item.AppendChild(e);
        }
        if (props.TryGetValue("Creator", out prop))
        {
            var e = ownerDocument.CreateElement("dc", "creator", NS_DC);
            e.InnerText = prop;
            item.AppendChild(e);
        }

        if (props.TryGetValue("Director", out prop))
        {
            var e = ownerDocument.CreateElement("upnp", "director", NS_UPNP);
            e.InnerText = prop;
            item.AppendChild(e);
        }
    }

    private void AddVideoProperties(IRequest request,
      IMediaResource resource,
      XmlNode item)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        var mvi = resource as IMetaVideoItem;
        if (mvi == null)
        {
            return;
        }
        try
        {
            var ownerDocument = item.OwnerDocument;
            var actors = mvi.MetaActors;
            if (actors != null && ownerDocument != null)
            {
                foreach (var actor in actors)
                {
                    var e = ownerDocument.CreateElement("upnp", "actor", NS_UPNP);
                    e.InnerText = actor;
                    item.AppendChild(e);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "AddVideoProperties failed");
        }
    }

    private static void Browse_AddFolder(XmlDocument result, IMediaFolder f)
    {
        var meta = f as IMetaInfo;
        var container = result.CreateElement(string.Empty, "container", NS_DIDL);
        container.SetAttribute("restricted", "0");
        container.SetAttribute("childCount", f.ChildCount.ToString());
        container.SetAttribute("id", f.Id);
        var parent = f.Parent;
        container.SetAttribute("parentID", parent == null ? Identifiers.GENERAL_ROOT : parent.Id);

        var title = result.CreateElement("dc", "title", NS_DC);
        title.InnerText = f.Title;
        container.AppendChild(title);
        if (meta != null)
        {
            var date = result.CreateElement("dc", "date", NS_DC);
            date.InnerText = meta.InfoDate.ToString("o");
            container.AppendChild(date);
        }

        var objectClass = result.CreateElement("upnp", "class", NS_UPNP);
        objectClass.InnerText = "object.container";
        container.AppendChild(objectClass);
        result.DocumentElement?.AppendChild(container);
    }

    private void Browse_AddItem(IRequest request, XmlDocument result,
      IMediaResource resource)
    {
        var props = resource.Properties;

        var item = result.CreateElement(string.Empty, "item", NS_DIDL);
        item.SetAttribute("restricted", "1");
        item.SetAttribute("id", resource.Id);
        item.SetAttribute("parentID", Identifiers.GENERAL_ROOT);

        item.AppendChild(CreateObjectClass(result, resource));

        AddBookmarkInfo(resource, item);

        AddGeneralProperties(props, item);

        AddVideoProperties(request, resource, item);

        var title = result.CreateElement("dc", "title", NS_DC);
        title.InnerText = resource.Title;
        item.AppendChild(title);

        var res = result.CreateElement(string.Empty, "res", NS_DIDL);
        res.InnerText =
          $"http://{request.LocalEndPoint.Address}:{request.LocalEndPoint.Port}{Prefix}file/{resource.Id}/res";

        string? prop;
        if (props.TryGetValue("SizeRaw", out prop))
        {
            res.SetAttribute("size", prop);
        }
        if (props.TryGetValue("Resolution", out prop))
        {
            res.SetAttribute("resolution", prop);
        }
        if (props.TryGetValue("Duration", out prop))
        {
            res.SetAttribute("duration", prop);
        }

        res.SetAttribute("protocolInfo", string.Format(
          "http-get:*:{1}:DLNA.ORG_PN={0};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS={2}",
          resource.PN, DlnaMaps.Mime[resource.Type], DlnaMaps.DefaultStreaming
                                           ));
        item.AppendChild(res);

        AddCover(request, resource, item);
        result.DocumentElement?.AppendChild(item);
    }

    private int BrowseFolder_AddItems(IRequest request, XmlDocument result,
      IMediaFolder root, int start,
      int requested)
    {
        var provided = 0;
        foreach (var i in root.ChildFolders)
        {
            if (start > 0)
            {
                start--;
                continue;
            }
            Browse_AddFolder(result, i);
            if (++provided == requested)
            {
                break;
            }
        }
        if (provided != requested)
        {
            foreach (var i in root.ChildItems)
            {
                if (start > 0)
                {
                    start--;
                    continue;
                }
                Browse_AddItem(request, result, i);
                if (++provided == requested)
                {
                    break;
                }
            }
        }
        return provided;
    }

    private static XmlNamespaceManager CreateNamespaceManager()
    {
        var rv = new XmlNamespaceManager(new NameTable());
        rv.AddNamespace("soap", NS_SOAPENV);
        return rv;
    }

    private static XmlElement CreateObjectClass(XmlDocument result,
      IMediaResource resource)
    {
        var objectClass = result.CreateElement("upnp", "class", NS_UPNP);
        switch (resource.MediaType)
        {
            case DlnaMediaTypes.Video:
                objectClass.InnerText = "object.item.videoItem.movie";
                break;

            case DlnaMediaTypes.Image:
                objectClass.InnerText = "object.item.imageItem.photo";
                break;

            case DlnaMediaTypes.Audio:
                objectClass.InnerText = "object.item.audioItem.musicTrack";
                break;

            default:
                throw new NotSupportedException();
        }
        return objectClass;
    }

    private IEnumerable<KeyValuePair<string, string?>> HandleBrowse(
      IRequest request, IHeaders sparams)
    {
        var key = Prefix + sparams.HeaderBlock;
        if (SoapCache.TryGetValue(key, out AttributeCollection? rv))
        {
            return rv;
        }

        var id = sparams["ObjectID"];
        var flag = sparams["BrowseFlag"];

        var requested = 20;
        var provided = 0;
        var start = 0;
        try
        {
            if (int.TryParse(sparams["RequestedCount"], out requested) &&
                requested <= 0)
            {
                requested = 20;
            }
            if (int.TryParse(sparams["StartingIndex"], out start) && start <= 0)
            {
                start = 0;
            }
        }
        catch (Exception ex)
        {
            Logger.LogDebug(ex, "Not all params provided");
        }

        if (GetItem(id) is not IMediaFolder root)
        {
            throw new ArgumentException("Invalid id");
        }
        var result = new XmlDocument();

        var didl = result.CreateElement(string.Empty, "DIDL-Lite", NS_DIDL);
        didl.SetAttribute("xmlns:dc", NS_DC);
        didl.SetAttribute("xmlns:dlna", NS_DLNA);
        didl.SetAttribute("xmlns:upnp", NS_UPNP);
        didl.SetAttribute("xmlns:sec", NS_SEC);
        result.AppendChild(didl);

        if (flag == "BrowseMetadata")
        {
            Browse_AddFolder(result, root);
            provided++;
        }
        else
        {
            provided = BrowseFolder_AddItems(
              request, result, root, start, requested);
        }
        var resXML = result.OuterXml;
        rv = new AttributeCollection
        {
            {"Result", resXML},
            {"NumberReturned", provided.ToString()},
            {"TotalMatches", root.ChildCount.ToString()},
            {"UpdateID", _systemID.ToString()}
        };
        SoapCache[key] = rv;
        return rv;
    }

    private static RawHeaders HandleGetCurrentConnectionIDs()
    {
        return new RawHeaders { { "ConnectionIDs", "0" } };
    }

    private static RawHeaders HandleGetCurrentConnectionInfo()
    {
        return new RawHeaders
        {
            {"RcsID", "-1"},
            {"AVTransportID", "-1"},
            {"ProtocolInfo", string.Empty},
            {"PeerConnectionmanager", string.Empty},
            {"PeerConnectionID", "0"},
            {"Direction", "Output"},
            {"Status", "OK"}
        };
    }

    private static RawHeaders HandleGetProtocolInfo()
    {
        return new RawHeaders
        {
            {"Source", DlnaMaps.ProtocolInfo},
            {"Sink", string.Empty}
        };
    }

    private static RawHeaders HandleGetSearchCapabilities()
    {
        return new RawHeaders { { "SearchCaps", string.Empty } };
    }

    private static RawHeaders HandleGetSortCapabilities()
    {
        return new RawHeaders { { "SortCaps", string.Empty } };
    }

    private IHeaders HandleGetSystemUpdateID()
    {
        return new RawHeaders { { "Id", _systemID.ToString() } };
    }

    private static RawHeaders HandleIsAuthorized()
    {
        return new RawHeaders { { "Result", "1" } };
    }

    private static RawHeaders HandleIsValidated()
    {
        return new RawHeaders { { "Result", "1" } };
    }

    private static RawHeaders HandleRegisterDevice()
    {
        return new RawHeaders { { "RegistrationRespMsg", string.Empty } };
    }

    private static RawHeaders HandleXGetFeatureList()
    {
        return new RawHeaders { { "FeatureList", featureList } };
    }

    private RawHeaders HandleXSetBookmark(IHeaders sparams)
    {
        var id = sparams["ObjectID"];
        var item = GetItem(id) as IBookmarkable;
        if (item != null)
        {
            var newbookmark = long.Parse(sparams["PosSecond"]);
            if (newbookmark > 30)
            {
                newbookmark -= 5;
            }
            if (newbookmark > 30 || !item.Bookmark.HasValue ||
                item.Bookmark.Value < 60)
            {
                item.Bookmark = newbookmark;
                SoapCache.Clear();
            }
        }
        return new RawHeaders();
    }

    private StringResponse ProcessSoapRequest(IRequest request)
    {
        if (string.IsNullOrEmpty(request.Body))
            throw new HttpStatusException(HttpCode.InternalError);

        var soap = new XmlDocument();
        soap.LoadXml(request.Body);
        var sparams = new RawHeaders();
        var body = soap.SelectSingleNode("//soap:Body", namespaceMgr);
        if (body == null)
        {
            throw new HttpStatusException(HttpCode.InternalError);
        }
        var method = body.FirstChild ?? throw new HttpStatusException(HttpCode.InternalError);
        foreach (var p in method.ChildNodes)
        {
            if (p is not XmlElement e)
            {
                continue;
            }
            sparams.Add(e.LocalName, e.InnerText.Trim());
        }
        var env = new XmlDocument();
        env.AppendChild(env.CreateXmlDeclaration("1.0", "utf-8", "yes"));
        var envelope = env.CreateElement("SOAP-ENV", "Envelope", NS_SOAPENV);
        env.AppendChild(envelope);
        envelope.SetAttribute(
          "encodingStyle", NS_SOAPENV,
          "http://schemas.xmlsoap.org/soap/encoding/");

        var rbody = env.CreateElement("SOAP-ENV:Body", NS_SOAPENV);
        env.DocumentElement?.AppendChild(rbody);

        var code = HttpCode.Ok;
        try
        {
            IEnumerable<KeyValuePair<string, string?>> result = method.LocalName switch
            {
                "GetSearchCapabilities" => HandleGetSearchCapabilities()!,
                "GetSortCapabilities" => HandleGetSortCapabilities()!,
                "GetSystemUpdateID" => HandleGetSystemUpdateID()!,
                "Browse" => HandleBrowse(request, sparams),
                "X_GetFeatureList" => HandleXGetFeatureList()!,
                "X_SetBookmark" => HandleXSetBookmark(sparams)!,
                "GetCurrentConnectionIDs" => HandleGetCurrentConnectionIDs()!,
                "GetCurrentConnectionInfo" => HandleGetCurrentConnectionInfo()!,
                "GetProtocolInfo" => HandleGetProtocolInfo()!,
                "IsAuthorized" => HandleIsAuthorized()!,
                "IsValidated" => HandleIsValidated()!,
                "RegisterDevice" => HandleRegisterDevice()!,
                _ => throw new HttpStatusException(HttpCode.NotFound),
            };
            var response = env.CreateElement($"u:{method.LocalName}Response", method.NamespaceURI);
            rbody.AppendChild(response);

            foreach (var i in result)
            {
                var ri = env.CreateElement(i.Key);
                ri.InnerText = i.Value!;
                response.AppendChild(ri);
            }
        }
        catch (Exception ex)
        {
            code = HttpCode.InternalError;
            var fault = env.CreateElement("SOAP-ENV", "Fault", NS_SOAPENV);
            var faultCode = env.CreateElement("faultcode");
            faultCode.InnerText = "500";
            fault.AppendChild(faultCode);
            var faultString = env.CreateElement("faultstring");
            faultString.InnerText = ex.ToString();
            fault.AppendChild(faultString);
            var detail = env.CreateDocumentFragment();
            detail.InnerXml =
              "<detail><UPnPError xmlns=\"urn:schemas-upnp-org:control-1-0\"><errorCode>401</errorCode><errorDescription>Invalid Action</errorDescription></UPnPError></detail>";
            fault.AppendChild(detail);
            rbody.AppendChild(fault);
            Logger.LogWarning(
              "Invalid call: Action: {action}, Params: {params}, Problem {problem}",
              method.LocalName, sparams, ex.Message);
        }

        var rv = new StringResponse(code, "text/xml", env.OuterXml);
        rv.Headers.Add("EXT", string.Empty);
        return rv;
    }
}