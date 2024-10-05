using Spectre.Console.Cli;

namespace Media.Tests.Autocomplete.Settings;

public sealed class AsynchronousCommandSettings : CommandSettings
{
    [CommandOption("--ThrowException")]
    [DefaultValue(false)]
    public bool ThrowException { get; set; }
}