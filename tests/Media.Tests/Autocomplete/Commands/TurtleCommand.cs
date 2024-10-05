using Media.Tests.Autocomplete.Settings;

namespace Media.Tests.Autocomplete.Commands;

[System.ComponentModel.Description("The turtle command.")]
public class TurtleCommand : AnimalCommand<TurtleSettings>
{
    public override int Execute(CommandContext context, TurtleSettings settings)
    {
        DumpSettings(context, settings);
        return 0;
    }
}
