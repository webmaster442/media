using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using FFCmd.FFMpegInterop;
using FFCmd.Infrastructure;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal class ExtractAudioCopy : BaseFFMpegCommand<ExtractAudioCopy.Settings>
{
    public class Settings : BaseFFMpegSettings
    {
        [Range(0, 999)]
        [Description("Audio stream index")]
        [CommandOption("-a|--audio-stream")]
        public int AudioStreamIndex { get; set; } = 0;
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioStreamSelection(settings.AudioStreamIndex)
            .WithAudioCodec("copy");
    }
}
