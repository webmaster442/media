using Media.Tests.Autocomplete.Settings;

namespace Media.Tests.Autocomplete.Commands;

[System.ComponentModel.Description("The user command.")]
internal class UserSuperAddCommand : Command<UserSuperAddSettings>
{
    public override int Execute(CommandContext context, UserSuperAddSettings settings)
    {
        return 0;
    }
}