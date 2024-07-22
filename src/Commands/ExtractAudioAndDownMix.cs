using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

using FFCmd.Infrastructure;
using FFCmd.Interop;

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

        [Range(0, 999)]
        [Description("Audio stream index")]
        [CommandOption("-a|--audio-stream")]
        public int AudioStreamIndex { get; set; } = 0;

        [Description("Volume level")]
        [CommandOption("-v|--volume")]
        [Range(1.0, 5.0)]
        public double Volume { get; set; } = 1.66;
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        string volumeValue = settings.Volume.ToString(CultureInfo.InvariantCulture);

        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioCodec("aac")
            .WithAudioBitrate(settings.Bitrate)
            .WithAudioStreamSelection(settings.AudioStreamIndex)
            .WithAudioFilter($"pan=stereo|c0=0.5*c2+0.707*c0+0.707*c4+0.5*c3|c1=0.5*c2+0.707*c1+0.707*c5+0.5*c3, volume={volumeValue}");
    }
}
