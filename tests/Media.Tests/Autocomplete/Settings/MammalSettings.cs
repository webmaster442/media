using Spectre.Console.Cli;

namespace Media.Tests.Autocomplete.Settings;

public class MammalSettings : AnimalSettings
{
    [CommandOption("-n|-p|--name|--pet-name <VALUE>")]
    public string Name { get; set; } = string.Empty;
}
