using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal sealed class BachRemove : BaseBachCommand<BachRemove.Settings>
{
    public class Settings : BaseBachSettings
    {
        [CommandArgument(0, "<file or file pattern>")]
        [Description("File or file pattern (eg. *.mp3) to remove from the project")]
        [Required]
        public string FileToAdd { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var project = await LoadProject(settings.ProjectName);
            var files = GetFiles(settings.FileToAdd);
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

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }
}
