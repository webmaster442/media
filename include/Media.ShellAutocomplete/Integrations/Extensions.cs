// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Spectre.Console.Cli;

namespace Media.ShellAutoComplete.Integrations;

public static class Extensions
{
    public static IConfigurator<CommandSettings> AddPowershell(this IConfigurator<CommandSettings> settings)
    {
        settings.AddDelegate<PowershellSettings>("powershell", (context, pwsh) =>
        {
            var settings = new PowershellIntegrationBuilderSettings
            {
                Install = pwsh.Install
            };

            var result = PowershellIntegrationBuilder.BuildIntegration(settings);
            Console.WriteLine(result);

            return 0;
        }).WithDescription("Powershell autocomplete integration");

        return settings;
    }

    private class PowershellSettings : CommandSettings
    {
        public bool Install { get; set; }
    }
}