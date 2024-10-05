namespace Media.Tests.Autocomplete.Settings;

public class LionSettings : CatSettings
{
    [CommandArgument(0, "<TEETH>")]
    [System.ComponentModel.Description("The number of teeth the lion has.")]
    public int Teeth { get; set; }

    [CommandOption("-c <CHILDREN>")]
    [System.ComponentModel.Description("The number of children the lion has.")]
    public int Children { get; set; }

    [CommandOption("-d <DAY>")]
    [System.ComponentModel.Description("The days the lion goes hunting.")]
    [DefaultValue(new[] { DayOfWeek.Monday, DayOfWeek.Thursday })]
    public DayOfWeek[] HuntDays { get; set; } = Array.Empty<DayOfWeek>();
}