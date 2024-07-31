// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Interop;

namespace Media.Commands;

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
            YtDlp.Start(arguments);

            return ExitCodes.Success;
        }
        catch (Exception ex)
        {
            Terminal.DisplayException(ex);
            return ExitCodes.Exception;
        }
    }
}