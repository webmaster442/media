// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;

internal sealed class MuxAddAudio : BaseFFMpegCommand<MuxAddAudio.Settings>
{
    public MuxAddAudio(IDryRunResultAcceptor dryRunResultAcceptor) : base(dryRunResultAcceptor)
    {
    }

    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".mkv";

        [FileExists]
        [Description("Audio file to mux in")]
        [CommandOption("-a|--audio-file")]
        public string AudioFile { get; set; } = "";

        [Required]
        [Description("Audio title metadata")]
        [CommandOption("-t|--title")]
        public string AudioTitle { get; set; } = "";
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .WithAudioCodec(FFMpeg.AudioCodecNames.Copy)
            .WithVideoCodec(FFMpeg.VideoCodecNames.Copy)
            .WithAdditionalInputFiles(settings.AudioFile)
            .WithAdditionalsBeforeOutputFile($"-map 0 -map 1:a -metadata:s:a:1 title=\"{settings.AudioTitle}\"");
    }
}