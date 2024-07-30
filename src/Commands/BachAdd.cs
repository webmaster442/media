using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

internal sealed class BachAdd : BaseBachCommand<BachAdd.Settings>
{
    public class Settings : BaseBachSettings
    {
        [CommandArgument(0, "<file or file pattern>")]
        [Description("File or file pattern (eg. *.mp3) to add to the project")]
        [Required]
        public string FileToAdd { get; set; } = string.Empty;
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        var project = await LoadProject(settings.ProjectName);
        var files = GetFiles(settings.FileToAdd);
        int count = project.Files.Count;
        project.Files.AddRange(files);
        count = project.Files.Count - count;
        await SaveProject(settings.ProjectName, project);

        Terminal.GreenText($"Added {count} files to project {settings.ProjectName}");
    }
}
