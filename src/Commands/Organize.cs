using System.Xml.Serialization;

using Media.Dto;
using Media.Dto.Organizer;
using Media.Embedded;
using Media.Infrastructure;
using Media.Infrastructure.Validation;

using Microsoft.Extensions.Logging;

namespace Media.Commands;

internal sealed class Organize : AsyncCommand<Organize.Settings>
{
    public class Settings : ValidatedCommandSettings
    {
        [DirectoryExists]
        [Description("Source directory to organize")]
        [CommandArgument(0, "<sorucedirectory>")]
        public string SoruceDirectory { get; set; } = Environment.CurrentDirectory;

        [CommandOption("-d|--destination")]
        [Description("Destination directory. Files will be moved here")]
        public string DestinationDirectory { get; set; } = string.Empty;

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

        foreach (Rule rule in rules)
        {
            var matches = rule.GetMathcingFiles(settings.SoruceDirectory).ToList();
            if (matches.Count > 0)
            {
                var targetDirectory = Path.Combine(settings.DestinationDirectory, rule.Folder);
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                logger.LogInformation("Moving {mathches} files to {targetDirectory}", matches.Count, targetDirectory);

                foreach (var file in matches)
                {
                    var targetFile = Path.Combine(targetDirectory, Path.GetFileName(file));
                    File.Move(file, targetFile);
                }
            }
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
