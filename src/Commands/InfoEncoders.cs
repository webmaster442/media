
using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Interop;

namespace Media.Commands;

internal class InfoEncoders : AsyncCommand<InfoEncoders.Settings>
{
    public class Settings : ValidatedCommandSettings
    {
        [CommandOption("-a|--audio")]
        [Description("List audio encoders")]
        public bool ListAudio { get; set; }

        [CommandOption("-v|--video")]
        [Description("List video encoders")]
        public bool ListVideo { get; set; }

        [CommandOption("-s|--subtitle")]
        [Description("List subtitle encoders")]
        public bool ListSubtitle { get; set; }

        public bool NoneGiven =>
            !ListAudio && !ListVideo && !ListSubtitle;
    }

    private readonly ConfigAccessor _configAccessor;

    public InfoEncoders()
    {
        _configAccessor = new ConfigAccessor();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var encoders = await GetEncoders();
        if (!settings.NoneGiven)
        {
            encoders = Filter(encoders, settings);
        }
        PrintTable(encoders);
        return ExitCodes.Success;
    }

    private FFMpegEncoderInfo[] Filter(FFMpegEncoderInfo[] encoders, Settings settings)
    {
        var filter = PredicateBuilder.False<FFMpegEncoderInfo>();
        if (settings.ListVideo)
        {
            filter = filter.Or(e => e.Type == FFMpegEncoderInfo.EncoderType.Video);
        }
        if (settings.ListAudio)
        {
            filter = filter.Or(e => e.Type == FFMpegEncoderInfo.EncoderType.Audio);
        }
        if (settings.ListSubtitle)
        {
            filter = filter.Or(e => e.Type == FFMpegEncoderInfo.EncoderType.Subtitle);
        }
        return encoders
            .Where(filter.Lambda())
            .ToArray();
    }

    private void PrintTable(FFMpegEncoderInfo[] encoders)
    {
        throw new NotImplementedException();
    }

    private async Task<FFMpegEncoderInfo[]> GetEncoders()
    {
        var installedVersion = _configAccessor.GetInstalledVersion("ffmpeg");
        if (installedVersion == null)
            throw new InvalidOperationException("Coudn't get installed version of ffmpeg");

        var cached = _configAccessor.GetCachedEncoderList();
        if (cached != null 
            && installedVersion == cached.Version)
        {
            return cached.Encoders;
        }

        var encoderString = FFMpeg.GetEnoderList();
        
        var parsed = Parsers.ParseEncoderInfos(encoderString).ToArray();

        await _configAccessor.SetCachedEncoderList(new Dto.Config.EncoderInfos
        { 
            Encoders = parsed,
            Version = installedVersion.Value,
        });

        return parsed;

    }
}
