﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

namespace Media.Commands;

[Example("Convert a file to Aplle losless", "media convert input.wav output.m4a")]
internal sealed class ConvertToAlac : BaseFFMpegCommand<ConvertToAlac.Settings>
{
    public ConvertToAlac(ConfigAccessor configAccessor)
        : base(configAccessor)
    {
    }

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
            .WithAudioCodec(FFMpeg.AudioCodecNames.Alac);
    }
}