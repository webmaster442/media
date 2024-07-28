using FFCmd.Infrastructure.Validation;

namespace FFCmd.Infrastructure;

internal class BaseFFMpegSettings : ValidatedCommandSettings
{
    [Description("Input file")]
    [CommandArgument(0, "<input>")]
    [FileExists]
    [Required]
    public string InputFile { get; init; }

    [Description("Output file")]
    [CommandArgument(1, "<output>")]
    [OutputFileHasExtension(nameof(OutputExtension))]
    [Required]
    public string OutputFile { get; init; }

    public virtual string OutputExtension { get; } = string.Empty;

    public BaseFFMpegSettings()
    {
        InputFile = string.Empty;
        OutputFile = string.Empty;
    }
}
