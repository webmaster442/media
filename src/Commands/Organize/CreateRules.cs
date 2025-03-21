using Media.Embedded;
using Media.Infrastructure;
using Media.Infrastructure.Validation;

namespace Media.Commands.Organize;

internal sealed class CreateRules : AsyncCommand<CreateRules.Settings>
{
    public class Settings : ValidatedCommandSettings
    {
        [DirectoryExists]
        [Description("Target directory to write rules to")]
        [CommandArgument(0, "<Directory>")]
        public string Directory { get; set; } = Environment.CurrentDirectory;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var fileName = Path.Combine(settings.Directory, EmbeddedResources.OrganizeRules);
        if (!File.Exists(fileName))
        {
            await EmbeddedResources.ExtractAsync(EmbeddedResources.OrganizeRules, settings.Directory);
        }

        return 0;
    }
}
