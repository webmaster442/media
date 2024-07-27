using System.ComponentModel;

using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;
using FFCmd.Interop;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal sealed class ConvertContactSheet : BaseFFMpegCommand<ConvertContactSheet.Settings>
{
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
