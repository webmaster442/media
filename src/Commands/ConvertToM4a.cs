using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;
using FFCmd.Interop;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal sealed class ConvertToM4a : BaseFFMpegCommand<ConvertToM4a.Settings>
{
    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".m4a";

        [Required]
        [Description("Audio bitrate")]
        [CommandOption("-b|--bitrate")]
        [AllowedValues("64k", "96k", "128k", "160k", "192k", "256k", "320k")]
        public string Bitrate { get; set; } = "";
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioCodec("aac")
            .WithAudioBitrate(settings.Bitrate);
    }
}
