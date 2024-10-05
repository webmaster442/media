using Spectre.Console.Cli;

namespace Media.Tests.Autocomplete.Settings;

public class ArgumentVectorSettings : CommandSettings
{
    [CommandArgument(0, "<Foos>")]
    public string[] Foo { get; set; } = Array.Empty<string>();
}
