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