// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

internal sealed class BachSetConversion : BaseBachCommand<BachSetConversion.Settings>
{
    public class Settings : BaseBachSettings
    {
        [CommandArgument(0, "<conversion command name>")]
        [Description("Preset name to set for the project")]
        [Required]
        public string Conversion { get; set; } = string.Empty;
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
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
    }
}
