// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.BaseCommands;
using Media.DbAdapters;
using Media.Infrastructure;
using Media.Infrastructure.Validation;

using Spectre.Console;

namespace Media.Commands.Config;

[Example("Set a configuration value", "media config set MpvRemotePort 8146")]
internal sealed class ConfigSet : BaseConfigCommand<ConfigSet.Settings>
{
    public class Settings : ValidatedCommandSettings
    {
        [Description("Property to set")]
        [CommandArgument(0, "<PROPERTY>")]
        [NotEmptyOrWiteSpace]
        public string Property { get; init; } = string.Empty;

        [Description("Value to set")]
        [CommandArgument(1, "<VALUE>")]
        [NotEmptyOrWiteSpace]
        public string Value { get; init; } = string.Empty;
    }

    public ConfigSet(ConfigAdapter configAdapter) : base(configAdapter)
    {
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        if (TrySetProperty(settings.Property, settings.Value))
        {
            return ExitCodes.Success;
        }

        AnsiConsole.MarkupLine("[red]Property not found or value is invalid[/]");
        return ExitCodes.Error;
    }
}
