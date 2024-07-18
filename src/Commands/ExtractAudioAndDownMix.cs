using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using FFCmd.FFMpegInterop;
using FFCmd.Infrastructure;

using Spectre.Console.Cli;

namespace FFCmd.Commands;
internal class ExtractAudioStereoM4a : BaseFFMpegCommand<ExtractAudioStereoM4a.Settings>
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
            .WithAudioBitrate(settings.Bitrate)
            .WithVolume(425)
            .WithAudioFilter("pan=stereo|c0=0.5*c2+0.707*c0+0.707*c4+0.5*c3|c1=0.5*c2+0.707*c1+0.707*c5+0.5*c3");
    }
}
