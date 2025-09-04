// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.BaseCommands;
using Media.DbAdapters;
using Media.Infrastructure.CommandAttributes;

using Spectre.Console;

namespace Media.Commands.Config;

[Example("List all configuration values", "media config list")]
internal sealed class ConfigList : BaseConfigCommand<ConfigList.Settings>
{
    public class Settings : CommandSettings;

    public ConfigList(ConfigAdapter configAdapter) : base(configAdapter)
    {
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(GetPropertiesTable());
        return ExitCodes.Success;
    }
}
