using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;

internal sealed class MuxAddSubtitle : BaseFFMpegCommand<MuxAddSubtitle.Settings>
{
    public MuxAddSubtitle(IDryRunResultAcceptor? dryRunResultAcceptor) : base(dryRunResultAcceptor)
    {
    }

    public class Settings : BaseFFMpegSettings
    {
        public override string OutputExtension => ".mkv";

        [FileExists]
        [Description("Subtitle file to mux in")]
        [CommandOption("-s|--subtitle-file")]
        public string SubtitleFile { get; set; } = "";

        [Required]
        [Description("Subtitle title metadata")]
        [CommandOption("-t|--title")]
        public string SubtitleTitle { get; set; } = "";
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .WithAudioCodec("copy")
            .WithVideoCodec("copy")
            .WithAdditionalInputFiles(settings.SubtitleFile)
            .WithAdditionalsBeforeOutputFile($"-c:s mov_text -metadata:s:s:0 title=\"{settings.SubtitleTitle}\"");
    }
}