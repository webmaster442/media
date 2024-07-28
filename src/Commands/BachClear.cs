using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;
using FFCmd.Infrastructure.Validation;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal sealed class BachClear : BaseBachCommand<BaseBachSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, BaseBachSettings settings)
    {
        try
        {
            var project = await LoadProject(settings.ProjectName);

            int count = project.Files.Count;
            project.Files.Clear();

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
