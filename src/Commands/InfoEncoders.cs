// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Interop;

using Spectre.Console;

namespace Media.Commands;

internal class InfoEncoders : Command<InfoEncoders.Settings>
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

    private readonly FFMpeg _ffMpeg;

    public InfoEncoders(ConfigAccessor configAccessor)
    {
        _ffMpeg = new FFMpeg(configAccessor);
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        try
        {
            IEnumerable<FFMpegEncoderInfo> encoders = _ffMpeg.GetEncoders().OrderBy(x => x.Name);

            if (!settings.NoneGiven)
            {
                encoders = Filter(encoders, settings);
            }
            PrintTable(encoders);
            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            return ExitCodes.Exception;
        }
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


}
