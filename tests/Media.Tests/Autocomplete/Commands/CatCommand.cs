using Media.Tests.Autocomplete.Settings;

namespace Media.Tests.Autocomplete.Commands;

public class CatCommand : AnimalCommand<CatSettings>
{
    public override int Execute(CommandContext context, CatSettings settings)
    {
        DumpSettings(context, settings);
        return 0;
    }
}
