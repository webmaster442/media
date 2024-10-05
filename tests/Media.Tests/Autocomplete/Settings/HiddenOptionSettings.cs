namespace Media.Tests.Autocomplete.Settings;

public sealed class HiddenOptionSettings : CommandSettings
{
    [CommandArgument(0, "<FOO>")]
    [System.ComponentModel.Description("Dummy argument FOO")]
    public int Foo { get; set; }

    [CommandOption("--bar", IsHidden = true)]
    [System.ComponentModel.Description("You should not be able to read this unless you used the 'cli explain' command with the '--hidden' option")]
    public int Bar { get; set; }

    [CommandOption("--baz")]
    [System.ComponentModel.Description("Dummy option BAZ")]
    public int Baz { get; set; }
}
