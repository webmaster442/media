using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Interfaces.Metadata;
using NMaier.SimpleDlna.Server.Types;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Responses;

internal sealed class ItemResponse : Logging, IResponse
{
    private readonly Headers _headers;

    private readonly IMediaResource _item;

    public ItemResponse(string prefix, IRequest request, IMediaResource item, ILoggerFactory loggerFactory, string transferMode = "Streaming")
        :base(loggerFactory)
    {
        this._item = item;
        _headers = new ResponseHeaders(item is not IMediaCoverResource);
        if (item is IMetaInfo meta)
        {
            _headers.Add("Content-Length", meta.InfoSize?.ToString() ?? "0");
            _headers.Add("Last-Modified", meta.InfoDate.ToString("R"));
        }
        _headers.Add("Accept-Ranges", "bytes");
        _headers.Add("Content-Type", DlnaMaps.Mime[item.Type]);
        if (request.Headers.ContainsKey("getcontentFeatures.dlna.org"))
        {
            try
            {
                _headers.Add(
                  "contentFeatures.dlna.org",
                  item.MediaType == DlnaMediaTypes.Image
                    ? $"DLNA.ORG_PN={item.PN};DLNA.ORG_OP=00;DLNA.ORG_CI=0;DLNA.ORG_FLAGS={DlnaMaps.DefaultInteractive}"
                    : $"DLNA.ORG_PN={item.PN};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS={DlnaMaps.DefaultStreaming}"
                  );
            }
            catch (NotSupportedException)
            {
            }
            catch (NotImplementedException)
            {
            }
        }
        if (request.Headers.ContainsKey("getCaptionInfo.sec"))
        {
            if (item is IMetaVideoItem mvi && mvi.Subtitle.HasSubtitle)
            {
                var surl =
                  $"http://{request.LocalEndPoint.Address}:{request.LocalEndPoint.Port}{prefix}subtitle/{item.Id}/st.srt";
                Logger.LogDebug("Sending subtitles {surl}", surl);
                _headers.Add("CaptionInfo.sec", surl);
            }
        }
        if (request.Headers.ContainsKey("getMediaInfo.sec"))
        {
            var md = item as IMetaDuration;
            if (md?.MetaDuration != null)
            {
                _headers.Add(
                  "MediaInfo.sec",
                  $"SEC_Duration={md.MetaDuration.Value.TotalMilliseconds};"
                  );
            }
        }
        _headers.Add("transferMode.dlna.org", transferMode);

        Logger.LogDebug(_headers.ToString());
    }

    public Stream Body => _item.CreateContentStream();

    public IHeaders Headers => _headers;

    public HttpCode Status { get; } = HttpCode.Ok;
}