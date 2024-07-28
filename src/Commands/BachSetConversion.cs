using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;

namespace FFCmd.Commands;

internal sealed class BachSetConversion : BaseBachCommand<BachSetConversion.Settings>
{
    public class Settings : BaseBachSettings
    {
        [CommandArgument(0, "<conversion command name>")]
        [Description("Preset name to set for the project")]
        [Required]
        public string Conversion { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            List<string> additionals = new();

            foreach (var arg in context.Remaining.Parsed)
            {
                additionals.Add(arg.Key.Length == 1 ? $"-{arg.Key}" : $"--{arg.Key}");
                additionals.AddRange(arg.Where(x => !string.IsNullOrEmpty(x))!);
            }

            var project = await LoadProject(settings.ProjectName);

            project.ConversionCommand = settings.Conversion;

            project.Args = additionals;

            await SaveProject(settings.ProjectName, project);

            Terminal.GreenText($"Set conversion command {settings.Conversion} for project {settings.ProjectName}");
            Terminal.GreenText("Additional specified args:");
            Terminal.GreenText($"{string.Join('\n', project.Args)}");

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }
}
