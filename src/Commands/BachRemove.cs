// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

internal sealed class BachRemove : BaseBachCommand<BachRemove.Settings>
{
    public class Settings : BaseBachSettings
    {
        [CommandArgument(0, "<file or file pattern>")]
        [Description("File or file pattern (eg. *.mp3) to remove from the project")]
        [Required]
        public string FileToRemove { get; set; } = string.Empty;
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        var project = await LoadProject(settings.ProjectName);
        var files = GetFiles(settings.FileToRemove);
        int count = 0;

        foreach (var file in files)
        {
            if (project.Files.Remove(file))
            {
                count++;
            }
        }

        await SaveProject(settings.ProjectName, project);

        Terminal.GreenText($"Removed {count} files from project {settings.ProjectName}");
    }
}
