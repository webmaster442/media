// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Interop;

namespace Media.Commands;

[Example("Download a video from youtube in HD 1080p (if available)", "media ytdload http://youtu.be/id -q Hd1080Mp4")]
internal class YtDload : AsyncCommand<YtDload.Settings>
{
    private readonly YtDlp _ytdlp;

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

    public YtDload(ConfigAccessor configAccessor)
    {
        _ytdlp = new YtDlp(configAccessor);
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            string formatTable = await _ytdlp.ExtractFromatTable(settings.Url);
            IEnumerable<Dto.Internals.YtDlpFormat> table = Parsers.ParseFormats(formatTable);
            var arguments = YtDlp.CreateDownloadArguments(table, settings.Quality, settings.Url);
            _ytdlp.Start(arguments);

            return ExitCodes.Success;
        }
        catch (Exception ex)
        {
            Terminal.DisplayException(ex);
            return ExitCodes.Exception;
        }
    }
}