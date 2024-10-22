// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

namespace Media.Commands;

[Example("Convert a video to a contact sheet", "media contactsheet --rows 5 --cols 5 input.mp4 output.jpg")]
internal sealed class ConvertContactSheet : BaseFFMpegCommand<ConvertContactSheet.Settings>
{
    public ConvertContactSheet(ConfigAccessor configAccessor)
        : base(configAccessor)
    {
    }

    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".jpg";

        [CommandOption("-r|--rows")]
        [Description("Contact sheet rows")]
        public int Rows { get; set; } = 10;

        [CommandOption("-c|--cols")]
        [Description("Contact sheet columns")]
        public int Cols { get; set; } = 10;
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .WithVideoFilter($"select=not(mod(n\\,100)),scale=320:-1,tile={settings.Cols}x{settings.Rows}")
            .WithVideoQuality(10)
            .WithVsync("vfr");
    }
}