using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

internal sealed class BachClear : BaseBachCommand<BaseBachSettings>
{
    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, BaseBachSettings settings)
    {
        var project = await LoadProject(settings.ProjectName);

        int count = project.Files.Count;
        project.Files.Clear();

        await SaveProject(settings.ProjectName, project);

        Terminal.GreenText($"Removed {count} files from project {settings.ProjectName}");
    }
}
