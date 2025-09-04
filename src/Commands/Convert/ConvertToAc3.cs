// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.BaseCommands;
using Media.BaseSettings;
using Media.DbAdapters;
using Media.Infrastructure.CommandAttributes;
using Media.Interop;

namespace Media.Commands.Convert;

[Example("Convert a file to Dolby Digital AC-3 with 256k bitrate", "media convert ac3 input.wav output.ac3 -b 256k")]
internal sealed class ConvertToAc3 : BaseFFMpegCommand<ConvertToAc3.Settings>
{
    public ConvertToAc3(ConfigAdapter configAccessor)
        : base(configAccessor)
    {
    }

    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".ac3";

        [Required]
        [Description("Audio bitrate. Valid values: 32k, 40k, 56k, 64k, 80k, 96k, 112k, 128k, 192k, 224k, 256k, 320k, 384k, 448k, 512k, 576k, 640k")]
        [CommandOption("-b|--bitrate")]
        [AllowedValues("32k", "40k", "48k", "56k", "64k", "80k", "96k", "112k", "128k", "160k", "192k", "224k", "256k", "320k", "384k", "448k", "512k", "576k", "640k")]
        public string Bitrate { get; set; } = "";
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .WithIgnoreVideo()
            .WithAudioCodec(FFMpeg.AudioCodecNames.Ac3)
            .WithAudioBitrate(settings.Bitrate);
    }
}
