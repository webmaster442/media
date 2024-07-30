// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure.Validation;

namespace Media.Infrastructure;

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
