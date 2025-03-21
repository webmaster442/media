// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Organizer;

using Microsoft.Extensions.Logging;

namespace Media.Infrastructure;

internal static class Organizer
{
    public static void ApplyOrganizeRules(IEnumerable<Rule> rules, ILogger logger, string sourceDirectory, string destinationDirectory)
    {
        foreach (Rule rule in rules)
        {
            var matches = rule.GetMathcingFiles(sourceDirectory).ToList();
            if (matches.Count > 0)
            {
                var targetDirectory = Path.Combine(destinationDirectory, rule.Folder);
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                logger.LogInformation("Moving {mathches} files to {targetDirectory}", matches.Count, targetDirectory);

                foreach (var file in matches)
                {
                    var targetFile = Path.Combine(targetDirectory, Path.GetFileName(file));
                    File.Move(file, targetFile);
                }
            }
        }
    }
}
