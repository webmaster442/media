using Media.Tests.Autocomplete.Settings;

namespace Media.Tests.Autocomplete.Commands;

public sealed class InvalidCommand : Command<InvalidSettings>
{
    public override int Execute(CommandContext context, InvalidSettings settings)
    {
        return 0;
    }
}
