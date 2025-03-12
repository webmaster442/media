using System.Xml.Serialization;

using Media.Dto;
using Media.Dto.Organizer;
using Media.Embedded;
using Media.Infrastructure;
using Media.Infrastructure.Validation;

namespace Media.Commands;

internal sealed class Organize : AsyncCommand<Organize.Settings>
{
    public class Settings : ValidatedCommandSettings
    {
        [DirectoryExists]
        [Description("Source directory to organize")]
        [CommandArgument(0, "<sorucedirectory>")]
        public string SoruceDirectory { get; set; } = Environment.CurrentDirectory;

        [DirectoryExists]
        [CommandOption("-d|--destination")]
        [Description("Destination directory. Files will be moved here")]
        public string DestinationDirectory { get; set; } = Environment.CurrentDirectory;

        [FileExists]
        [Description("Rule file to use")]
        [CommandOption("-r|--rules")]
        public string RuleFile { get; set; } = string.Empty;

        public string GetRuleFile()
        {
            if (!string.IsNullOrEmpty(RuleFile))
                return Path.GetFullPath(RuleFile);

            var destinationRules = Path.Combine(Path.GetFullPath(DestinationDirectory), EmbeddedResources.OrganizeRules);
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

        Rule[] rules = LoadFile(settings.GetRuleFile());

        foreach (var rule in rules)
        {
            var matches = rule.GetMathcingFiles(settings.SoruceDirectory).ToList();
        }

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
