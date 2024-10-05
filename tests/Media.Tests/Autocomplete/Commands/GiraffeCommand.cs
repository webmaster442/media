using Media.Tests.Autocomplete.Settings;

namespace Media.Tests.Autocomplete.Commands;

[System.ComponentModel.Description("The giraffe command.")]
public sealed class GiraffeCommand : Command<GiraffeSettings>
{
    public override int Execute(CommandContext context, GiraffeSettings settings)
    {
        return 0;
    }
}
