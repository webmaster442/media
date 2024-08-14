// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure.Validation;

namespace Media.Infrastructure;

internal class BaseCdSettings : ValidatedCommandSettings
{
    [Required]
    [DriveLetter]
    [CommandArgument(0, "<cd drive letter>")]
    [Description("The drive letter of the cd drive to rip")]
    public string DriveLetter { get; set; } = string.Empty;
}