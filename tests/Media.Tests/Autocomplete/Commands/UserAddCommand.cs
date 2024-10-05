using Media.Tests.Autocomplete.Settings;

namespace Media.Tests.Autocomplete.Commands;

[System.ComponentModel.Description("The user command.")]
internal class UserAddCommand : Command<UserAddSettings>
{
    public override int Execute(CommandContext context, UserAddSettings settings)
    {
        return 0;
    }
}