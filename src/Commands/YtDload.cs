using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using FFCmd.Infrastructure;
using FFCmd.Interop;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal class YtDload : AsyncCommand<YtDload.Settings>
{
    public class Settings : ValidatedCommandSettings
    {
        [Required]
        [Description("URL to download")]
        [CommandArgument(0, "<URL>")]
        public string Url { get; init; } = string.Empty;

        [Description("Video Quality")]
        [CommandOption("-q|--quality")]
        public YtDlpQuality Quality { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            string formatTable = await YtDlp.ExtractFromatTable(settings.Url);
            IEnumerable<Dto.Internals.YtDlpFormat> table = Parsers.ParseFormats(formatTable);
            var arguments = YtDlp.CreateDownloadArguments(table, settings.Quality, settings.Url);
            YtDlp.StartYtDlp(arguments);

            return ExitCodes.Success;
        }
        catch (Exception ex)
        {
            Terminal.DisplayException(ex);
            return ExitCodes.Exception;
        }
    }
}