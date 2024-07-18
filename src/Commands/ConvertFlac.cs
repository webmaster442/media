using FFCmd.FFMpegInterop;
using FFCmd.Infrastructure;

namespace FFCmd.Commands;

internal class ConvertFlac : BaseFFMpegCommand<ConvertFlac.ConvertFlacSettings>
{
    protected override void BuildCommandLine(FFMpegCommandBuilder builder, ConvertFlacSettings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile);
    }

    public class ConvertFlacSettings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".flac";
    }

}
