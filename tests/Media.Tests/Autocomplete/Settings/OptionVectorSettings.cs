namespace Media.Tests.Autocomplete.Settings;

public class OptionVectorSettings : CommandSettings
{
    [CommandOption("--foo")]
    public string[] Foo { get; set; } = Array.Empty<string>();

    [CommandOption("--bar")]
    public int[] Bar { get; set; } = Array.Empty<int>();
}
