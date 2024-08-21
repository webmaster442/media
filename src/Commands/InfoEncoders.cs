// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Interop;

using Spectre.Console;

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

        [CommandOption("-w|--hardware")]
        [Description("List hardware accelerated encoders only")]
        public bool ListHardware { get; set; }

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
        IEnumerable<FFMpegEncoderInfo> encoders = (await GetEncoders()).OrderBy(x => x.Name);
        
        if (settings.ListHardware)
        {
            encoders = encoders.Where(e => IsHardWareAccelerated(e));
        }
        else if (!settings.NoneGiven)
        {
            encoders = Filter(encoders, settings);
        }
        PrintTable(encoders);
        return ExitCodes.Success;
    }

    private static bool IsHardWareAccelerated(FFMpegEncoderInfo e)
    {
        return e.Name.EndsWith("_amf")
            || e.Name.EndsWith("_nvenc")
            || e.Name.EndsWith("_qsv");
    }

    private static IEnumerable<FFMpegEncoderInfo> Filter(IEnumerable<FFMpegEncoderInfo> encoders, Settings settings)
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
        return encoders.Where(filter.Compile());
    }

    private static void PrintTable(IEnumerable<FFMpegEncoderInfo> encoders)
    {
        var table = new Table();
        table.AddColumns("Name", "Type", "Description");
        foreach (var encoder in encoders)
        {
            table.AddRow(encoder.Name, encoder.Type.ToString(), encoder.Description);
        }
        AnsiConsole.Write(table);
    }

    private async Task<FFMpegEncoderInfo[]> GetEncoders()
    {
        var installedVersion = _configAccessor.GetInstalledVersion("FFMpeg");
        if (installedVersion == null)
            throw new InvalidOperationException("Coudn't get installed version of FFMpeg");

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
