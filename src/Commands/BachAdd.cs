using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;

namespace FFCmd.Commands;

internal sealed class BachAdd : BaseBachCommand<BachAdd.Settings>
{
    public class Settings : BaseBachSettings
    {
        [CommandArgument(0, "<file or file pattern>")]
        [Description("File or file pattern (eg. *.mp3) to add to the project")]
        [Required]
        public string FileToAdd { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var project = await LoadProject(settings.ProjectName);
            var files = GetFiles(settings.FileToAdd);
            int count = project.Files.Count;
            project.Files.AddRange(files);
            count = project.Files.Count - count;
            await SaveProject(settings.ProjectName, project);

            Terminal.GreenText($"Added {count} files to project {settings.ProjectName}");

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }
}
