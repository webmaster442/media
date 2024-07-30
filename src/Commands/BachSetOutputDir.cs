// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Infrastructure.Validation;

namespace Media.Commands;

internal sealed class BachSetOutputDir : BaseBachCommand<BachSetOutputDir.Settings>
{
    public class Settings : BaseBachSettings
    {
        [CommandArgument(0, "<output directory name>")]
        [Description("Output directory")]
        [DirectoryExists]
        public string OutputDirectory { get; set; } = string.Empty;
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        var project = await LoadProject(settings.ProjectName);

        project.OutputDirectory = settings.OutputDirectory;

        await SaveProject(settings.ProjectName, project);

        Terminal.GreenText($"Set output directory {settings.OutputDirectory} for project {settings.ProjectName}");
    }
}