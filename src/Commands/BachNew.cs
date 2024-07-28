using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;

using Spectre.Console.Cli;
using FFCmd.Dto;

namespace FFCmd.Commands;

internal sealed class BachNew : BaseBachCommand<BachNew.Settings>
{
    internal class Settings : ValidatedCommandSettings
    {
        [Description("Bach project file")]
        [CommandOption("-p|--project")]
        [Required]
        public string ProjectName { get; set; }

        public Settings()
        {
            var files = Directory.GetFiles(Environment.CurrentDirectory, "*.bach");
            ProjectName = files.Length == 0 ? MakeProjectName(Environment.CurrentDirectory) : string.Empty;
        }

        private static string MakeProjectName(string currentDirectory)
        {
            var name = Path.GetFileName(currentDirectory);
            return Path.Combine(currentDirectory, $"{Path.ChangeExtension(name, ".bach")}");
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var project = new BachProject();

            await SaveProject(settings.ProjectName, project);

            Terminal.GreenText($"Created project {settings.ProjectName}");

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }
}