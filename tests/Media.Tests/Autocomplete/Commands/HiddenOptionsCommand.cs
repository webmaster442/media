using Media.Tests.Autocomplete.Settings;

namespace Media.Tests.Autocomplete.Commands;

public sealed class HiddenOptionsCommand : Command<HiddenOptionSettings>
{
    public override int Execute(CommandContext context, HiddenOptionSettings settings)
    {
        return 0;
    }
}
