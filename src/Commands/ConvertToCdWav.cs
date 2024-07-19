using FFCmd.Interop;
using FFCmd.Infrastructure;

namespace FFCmd.Commands;

internal sealed class ConvertToCdWav : BaseFFMpegCommand<ConvertToCdWav.Settings>
{
    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".wav";
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioCodec("pcm_s16le")
            .WithAudioSampleRate(44100);
    }
}