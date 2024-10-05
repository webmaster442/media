namespace Media.Tests.Autocomplete.Settings;

public class MultipleArgumentVectorSettings : CommandSettings
{
    [CommandArgument(0, "<Foos>")]
    public string[] Foo { get; set; } = Array.Empty<string>();

    [CommandArgument(0, "<Bars>")]
    public string[] Bar { get; set; } = Array.Empty<string>();
}

public class MultipleArgumentVectorSpecifiedFirstSettings : CommandSettings
{
    [CommandArgument(1, "[Bar]")]
    public string Bar { get; set; } = string.Empty;

    [CommandArgument(0, "<Foos>")]
    public string[] Foo { get; set; } = Array.Empty<string>();
}
