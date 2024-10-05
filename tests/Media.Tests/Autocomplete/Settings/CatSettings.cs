using Media.Tests.Autocomplete.Converters;
using Media.Tests.Autocomplete.Validators;

namespace Media.Tests.Autocomplete.Settings;

public class CatSettings : MammalSettings
{
    [CommandOption("--agility <VALUE>")]
    [TypeConverter(typeof(CatAgilityConverter))]
    [DefaultValue(10)]
    [System.ComponentModel.Description("The agility between 0 and 100.")]
    [PositiveNumberValidator("Agility cannot be negative.")]
    public int Agility { get; set; }
}
