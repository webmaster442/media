namespace Media.Tests.Autocomplete.Settings;

public sealed class InvalidSettings : CommandSettings
{
    [CommandOption("-f|--foo [BAR]")]
    public string Value { get; set; } = string.Empty;
}
