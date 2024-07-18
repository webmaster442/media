using FFCmd.FFMpegInterop;
using FFCmd.Infrastructure;

namespace FFCmd.Commands;

internal class ExtractAudioCopy : BaseFFMpegCommand<ExtractAudioCopy.Settings>
{
    public class Settings : BaseFFMpegSettings
    {
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioCodec("copy");
    }
}
