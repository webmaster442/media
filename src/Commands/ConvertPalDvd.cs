// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Interop;

namespace Media.Commands;

[Example("Convert a video file to PAL DVD compatible MPEG2 with an AC3 audio and 192k audio bitrate", "media convert dvd-pal input.mp4 output.mpg -b 192k")]
[Example("Convert a video file to PAL DVD compatible MPEG2 with letterbox (4:3) aspect", "media convert dvd-pal input.mp4 output.mpg -b 192k -l")]
internal sealed class ConvertPalDvd : BaseFFMpegCommand<ConvertPalDvd.Settings>
{
    public ConvertPalDvd(ConfigAccessor configAccessor, IDryRunResultAcceptor dryRunResultAcceptor)
        : base(configAccessor, dryRunResultAcceptor)
    {
    }

    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".mpg";

        [Required]
        [Description("Audio bitrate. Valid values: 32k, 40k, 56k, 64k, 80k, 96k, 112k, 128k, 192k, 224k, 256k, 320k, 384k, 448k, 512k, 576k, 640k")]
        [CommandOption("-b|--bitrate")]
        [AllowedValues("32k", "40k", "48k", "56k", "64k", "80k", "96k", "112k", "128k", "160k", "192k", "224k", "256k", "320k", "384k", "448k", "512k", "576k", "640k")]
        public string Bitrate { get; set; } = "";

        [Description("Generate a 4:3 video. If not specified 16:9 aspect is used")]
        [CommandOption("-l|--letterbox")]
        public bool AspectRatioIs4by3 { get; set; }
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .WithAudioCodec(FFMpeg.AudioCodecNames.Ac3)
            .WithAudioBitrate(settings.Bitrate)
            .WithAspectRatio(settings.AspectRatioIs4by3 ? "4:3" : "16:9")
            .WithTarget(FFMpeg.TargetNames.PalDvd);
    }
}
