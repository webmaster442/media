using Media.Tests.Autocomplete.Settings;

namespace Media.Tests.Autocomplete.Commands;

public class OptionVectorCommand : Command<OptionVectorSettings>
{
    public override int Execute(CommandContext context, OptionVectorSettings settings)
    {
        return 0;
    }
}
