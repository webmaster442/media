﻿using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;
using FFCmd.Interop;

namespace FFCmd.Commands;

internal sealed class ConvertToCdWav : BaseFFMpegCommand<ConvertToCdWav.Settings>
{
    public ConvertToCdWav(IDryRunResultAcceptor? dryRunResultAcceptor) : base(dryRunResultAcceptor)
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
            .WithAudioCodec("pcm_s16le")
            .WithAudioSampleRate(44100);
    }
}