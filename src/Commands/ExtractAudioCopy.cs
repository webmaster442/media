using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

namespace Media.Commands;

internal sealed class ExtractAudioCopy : BaseFFMpegCommand<ExtractAudioCopy.Settings>
{
    public ExtractAudioCopy(IDryRunResultAcceptor? dryRunResultAcceptor) : base(dryRunResultAcceptor)
    {
    }

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