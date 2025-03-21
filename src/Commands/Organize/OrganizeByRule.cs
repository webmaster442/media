using System.Xml.Serialization;

using Media.Dto.Organizer;
using Media.Embedded;
using Media.Infrastructure;
using Media.Infrastructure.Validation;

namespace Media.Commands.Organize;

internal sealed class OrganizeByRule : AsyncCommand<OrganizeByRule.Settings>
{
    public class Settings : OrganizeSettingsBase
    {
        [FileExists]
        [Description("Rule file to use")]
        [CommandOption("-r|--rules")]
        public string RuleFile { get; set; } = string.Empty;

        public string GetRuleFile()
        {
            if (!string.IsNullOrEmpty(RuleFile))
                return Path.GetFullPath(RuleFile);

            var destinationRules = Path.Combine(DestinationDirectory, EmbeddedResources.OrganizeRules);
            if (File.Exists(destinationRules))
                return destinationRules;

            return Path.Combine(AppContext.BaseDirectory, EmbeddedResources.OrganizeRules);
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var fileName = Path.Combine(AppContext.BaseDirectory, EmbeddedResources.OrganizeRules);
        if (!File.Exists(fileName))
        {
            await EmbeddedResources.ExtractAsync(EmbeddedResources.OrganizeRules);
        }

        using var loggerFactory = ProgramFactory.GetLoggerFactory();
        var logger = loggerFactory.CreateLogger("Organize");

        if (string.IsNullOrEmpty(settings.DestinationDirectory))
            settings.DestinationDirectory = Path.GetFullPath(settings.SoruceDirectory);

        Rule[] rules = LoadFile(settings.GetRuleFile());

        Organizer.ApplyOrganizeRules(rules, logger, settings.SoruceDirectory, settings.DestinationDirectory);

        return 0;
    }

    private Rule[] LoadFile(string file)
    {
        XmlSerializer xs = new XmlSerializer(typeof(Rule[]), new XmlRootAttribute("Rules"));
        using var fs = File.OpenRead(file);
        if (xs.Deserialize(fs) is Rule[] deserialized)
        {
            return deserialized;
        }
        throw new InvalidOperationException("Failed to deserialize rules");
    }
}
