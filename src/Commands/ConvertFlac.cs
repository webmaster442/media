using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using FFCmd.FFMpegInterop;
using FFCmd.Infrastructure;

namespace FFCmd.Commands;

internal class ConvertFlac : BaseFFMpegCommand<ConvertFlac.ConvertFlacSettings>
{
    protected override void BuildCommandLine(FFMpegCommandBuilder builder, ConvertFlacSettings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithCompressionLevel(settings.CompressionLevel);
    }

    public class ConvertFlacSettings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".flac";

        [Description("Compression level (1-12)")]
        [Range(1, 12)]
        public int CompressionLevel { get; init; } = 8;
    }

}
