using Media.Tests.Autocomplete.Settings;

namespace Media.Tests.Autocomplete.Commands;

public sealed class ThrowingCommand : Command<ThrowingCommandSettings>
{
    public override int Execute(CommandContext context, ThrowingCommandSettings settings)
    {
        throw new InvalidOperationException("W00t?");
    }
}