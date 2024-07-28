using System.ComponentModel;

using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;
using FFCmd.Infrastructure.Validation;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal sealed class BachSetOutputDir : BaseBachCommand<BachSetOutputDir.Settings>
{
    public class Settings : BaseBachSettings
    {
        [CommandArgument(0, "<output directory name>")]
        [Description("Output directory")]
        [DirectoryExists]
        public string OutputDirectory { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var project = await LoadProject(settings.ProjectName);

            project.OutputDirectory = settings.OutputDirectory;

            await SaveProject(settings.ProjectName, project);

            Terminal.GreenText($"Set output directory {settings.OutputDirectory} for project {settings.ProjectName}");

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }
}