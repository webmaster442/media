// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

namespace Media.Commands;

internal sealed class ConvertToCdWav : BaseFFMpegCommand<ConvertToCdWav.Settings>
{
    public ConvertToCdWav(IDryRunResultAcceptor dryRunResultAcceptor) : base(dryRunResultAcceptor)
    {
    }

    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".wav";
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioCodec(FFMpeg.AudioCodecNames.PcmS16Le)
            .WithAudioSampleRate(44100);
    }
}
