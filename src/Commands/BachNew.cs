// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto;
using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

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

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        var project = new BachProject();

        await SaveProject(settings.ProjectName, project);

        Terminal.GreenText($"Created project {settings.ProjectName}");
    }
}