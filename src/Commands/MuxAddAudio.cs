using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using FFCmd.Infrastructure;
using FFCmd.Infrastructure.Validation;
using FFCmd.Interop;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal sealed class MuxAddAudio : BaseFFMpegCommand<MuxAddAudio.Settings>
{
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
            .WithAudioCodec("copy")
            .WithVideoCodec("copy")
            .WithAdditionalInputFiles(settings.AudioFile)
            .WithAdditionalsBeforeOutputFile($"-map 0 -map 1:a -metadata:s:a:1 title=\"{settings.AudioTitle}\"");
    }
}
