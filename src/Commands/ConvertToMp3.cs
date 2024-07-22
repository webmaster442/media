using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using FFCmd.Infrastructure;
using FFCmd.Interop;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal sealed class ConvertToMp3 : BaseFFMpegCommand<ConvertToMp3.Settings>
{
    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".mp3";

        [Required]
        [Description("Audio bitrate")]
        [CommandOption("-b|--bitrate")]
        [AllowedValues("32k", "40k", "48k", "56k", "64k", "80k", "96k", "112k", "128k", "160k", "192k", "224k", "256k", "320k")]
        public string Bitrate { get; set; } = "";
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioBitrate(settings.Bitrate);
    }
}
