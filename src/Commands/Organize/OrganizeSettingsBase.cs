// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.Validation;

namespace Media.Commands.Organize;

internal class OrganizeSettingsBase : ValidatedCommandSettings
{
    [DirectoryExists]
    [Description("Source directory to organize")]
    [CommandArgument(0, "<sorucedirectory>")]
    public string SoruceDirectory { get; set; } = Environment.CurrentDirectory;

    [CommandOption("-d|--destination")]
    [Description("Destination directory. Files will be moved here")]
    public string DestinationDirectory { get; set; } = string.Empty;
}
