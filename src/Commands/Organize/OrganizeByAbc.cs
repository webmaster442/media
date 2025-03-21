// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Organizer;
using Media.Infrastructure;
using Media.Infrastructure.Validation;

namespace Media.Commands.Organize;

internal sealed class OrganizeByAbc : Command<OrganizeSettingsBase>
{
    public override int Execute(CommandContext context, OrganizeSettingsBase settings)
    {
        using var loggerFactory = ProgramFactory.GetLoggerFactory();
        var logger = loggerFactory.CreateLogger("Organize");

        if (string.IsNullOrEmpty(settings.DestinationDirectory))
            settings.DestinationDirectory = Path.GetFullPath(settings.SoruceDirectory);

        IEnumerable<Rule> rules = GenerateRules();

        Organizer.ApplyOrganizeRules(rules, logger, settings.SoruceDirectory, settings.DestinationDirectory);

        return 0;
    }

    private static IEnumerable<Rule> GenerateRules()
    {
        foreach (char letter in "abcdefghijklmnopqrstuvwxyz0123456789")
        {
            yield return new Rule
            {
                Folder = $"{letter}",
                Patterns = [new Pattern
                {
                    IgnoreCase = true,
                    IsRegex = true,
                    Value = $"^{letter}.*"
                }]
            };
        }
    }
}
