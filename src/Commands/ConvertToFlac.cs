// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

namespace Media.Commands;

[Example("Convert a file to flac", "media convert input.wav output.flac")]
[Example("Convert a file to flac with maximal compression", "media convert input.wav output.flac -c 12")]
internal sealed class ConvertToFlac : BaseFFMpegCommand<ConvertToFlac.Settings>
{
    public ConvertToFlac(ConfigAccessor configAccessor)
        : base(configAccessor)
    {
    }

    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".flac";

        [Description("Compression level (1-12)")]
        [CommandOption("-c|--compression")]
        [Range(1, 12)]
        public int CompressionLevel { get; init; } = 8;
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .WithAudioCodec(FFMpeg.AudioCodecNames.Flac)
            .IgnoreVideo()
            .WithCompressionLevel(settings.CompressionLevel);
    }
}