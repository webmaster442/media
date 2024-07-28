using FFCmd.Infrastructure.Validation;

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
