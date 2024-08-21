// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

namespace Media.Commands;

internal sealed class ConvertToM4a : BaseFFMpegCommand<ConvertToM4a.Settings>
{
    public ConvertToM4a(ConfigAccessor configAccessor, IDryRunResultAcceptor dryRunResultAcceptor)
        : base(configAccessor, dryRunResultAcceptor)
    {
    }

    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".m4a";

        [Required]
        [Description("Audio bitrate Valid values: 64k, 96k, 128k, 160k, 192k, 256k, 320k")]
        [CommandOption("-b|--bitrate")]
        [AllowedValues("64k", "96k", "128k", "160k", "192k", "256k", "320k")]
        public string Bitrate { get; set; } = "";
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioCodec(FFMpeg.AudioCodecNames.Aac)
            .WithAudioBitrate(settings.Bitrate);
    }
}