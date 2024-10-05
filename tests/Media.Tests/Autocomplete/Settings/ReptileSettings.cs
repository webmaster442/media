namespace Media.Tests.Autocomplete.Settings;

public class ReptileSettings : AnimalSettings
{
    [CommandOption("-n|-p|--name|--pet-name <VALUE>")]
    public string Name { get; set; } = string.Empty;
}