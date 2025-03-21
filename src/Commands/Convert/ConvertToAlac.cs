// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.BaseCommands;
using Media.BaseSettings;
using Media.DbAdapters;
using Media.Infrastructure.CommandAttributes;
using Media.Interop;

namespace Media.Commands.Convert;

[Example("Convert a file to Aplle losless", "media convert input.wav output.m4a")]
internal sealed class ConvertToAlac : BaseFFMpegCommand<ConvertToAlac.Settings>
{
    public ConvertToAlac(ConfigAdapter configAccessor)
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
            .WithIgnoreVideo()
            .WithAudioCodec(FFMpeg.AudioCodecNames.Alac);
    }
}