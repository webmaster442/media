// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Interop;

namespace Media.Commands;

internal sealed class ConvertToFlac : BaseFFMpegCommand<ConvertToFlac.Settings>
{
    public ConvertToFlac(ConfigAccessor configAccessor, IDryRunResultAcceptor dryRunResultAcceptor)
        : base(configAccessor, dryRunResultAcceptor)
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