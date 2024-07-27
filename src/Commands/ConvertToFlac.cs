using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;
using FFCmd.Interop;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal sealed class ConvertToFlac : BaseFFMpegCommand<ConvertToFlac.Settings>
{
    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".flac";

        [Description("Compression level (1-12)")]
        [CommandOption("-c|--compression")]
        [Range(1, 12)]
        public int CompressionLevel { get; init; } = 8;
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithCompressionLevel(settings.CompressionLevel);
    }
}
