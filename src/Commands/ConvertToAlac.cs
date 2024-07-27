using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;
using FFCmd.Interop;

namespace FFCmd.Commands;

internal sealed class ConvertToAlac : BaseFFMpegCommand<ConvertToAlac.Settings>
{
    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".m4a";
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioCodec("alac");
    }
}