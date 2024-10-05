namespace Media.Tests.Autocomplete.Commands;

public sealed class EmptyCommand : Command<EmptyCommandSettings>
{
    public override int Execute(CommandContext context, EmptyCommandSettings settings)
    {
        return 0;
    }
}
