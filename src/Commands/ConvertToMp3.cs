﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

namespace Media.Commands;

[Example("Convert a file to mp3 with 320k bitrate", "media convert mp3 input.wav output.mp3 -b 320k")]
internal sealed class ConvertToMp3 : BaseFFMpegCommand<ConvertToMp3.Settings>
{
    public ConvertToMp3(ConfigAccessor configAccessor)
        : base(configAccessor)
    {
    }

    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".mp3";

        [Required]
        [Description("Audio bitrate Valid values: 32k, 40k, 48k, 64k, 80k, 96k, 112k, 128k, 160k, 192k, 224k, 256k, 320k")]
        [CommandOption("-b|--bitrate")]
        [AllowedValues("32k", "40k", "48k", "56k", "64k", "80k", "96k", "112k", "128k", "160k", "192k", "224k", "256k", "320k")]
        public string Bitrate { get; set; } = "";
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioBitrate(settings.Bitrate);
    }
}
