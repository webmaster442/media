using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using FFCmd.Infrastructure.Validation;

using Spectre.Console.Cli;

namespace FFCmd.Infrastructure;

internal class BaseBachSettings : ValidatedCommandSettings
{
    [Description("Bach project file")]
    [CommandOption("-p|--project")]
    [Required]
    [FileExists]
    public string ProjectName { get; set; }

    public BaseBachSettings()
    {
        var files = Directory.GetFiles(Environment.CurrentDirectory, "*.bach");
        ProjectName = files.Length == 1 ? files[0] : string.Empty;
    }
}
