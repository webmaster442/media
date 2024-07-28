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
        public string FileToRemvoe { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var project = await LoadProject(settings.ProjectName);
            var files = GetFiles(settings.FileToRemvoe);
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
