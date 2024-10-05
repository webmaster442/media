namespace Media.Tests.Autocomplete.Settings;

public sealed class GiraffeSettings : MammalSettings
{
    [CommandArgument(0, "<LENGTH>")]
    [System.ComponentModel.Description("The option description.")]
    public int Length { get; set; }
}
