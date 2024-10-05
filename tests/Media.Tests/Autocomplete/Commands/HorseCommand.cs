using Media.Tests.Autocomplete.Settings;

namespace Media.Tests.Autocomplete.Commands;

[System.ComponentModel.Description("The horse command.")]
public class HorseCommand : AnimalCommand<HorseSettings>
{
    public override int Execute(CommandContext context, HorseSettings settings)
    {
        DumpSettings(context, settings);
        return 0;
    }
}
