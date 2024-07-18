using FFCmd.FFMpegInterop;
using FFCmd.Infrastructure;

using Spectre.Console;
using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal sealed class UpdateFFMpeg : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        try
        {
            AnsiConsole.WriteLine("Checking for ffmpleg update...");
            using var client = new GithubClient();
            var releases = await client.GetReleases("BtbN", "FFmpeg-Builds");

            var latest = releases
                .Where(r => !r.Prerelease
                    && !r.Draft
                    && r.Assets.Length > 0)
                .OrderByDescending(r => r.PublishedAt)
                .First();

            DateTimeOffset? installed = await FFMpeg.GetInstalledVersion();

            if (installed == null
                || installed < latest.PublishedAt)
            {
                var names = latest.Assets.Select(a => a.Name);

                var asset = latest.Assets
                    .Where(a => a.Name.Contains("win64-gpl-shared.zip"))
                    .First();

                string tempName = string.Empty;

                await AnsiConsole.Progress().AutoRefresh(false).StartAsync(async ctx =>
                {
                    var task1 = ctx.AddTask("Downloading ffmpeg...");
                    tempName = await client.DownloadAsset(asset, (long position, long length) =>
                    {
                        task1.Value = ((double)position / length) * 100;
                        ctx.Refresh();
                    });

                    var task2 = ctx.AddTask("Extracting ffmpeg...");

                    await FFMpegExtractor.ExtractBinariesTo("a:\\ffmpeg-master-latest-win64-gpl-shared.zip", AppContext.BaseDirectory, (long pogress, long total) =>
                    {
                        task2.Value = ((double)pogress / total) * 100;
                        ctx.Refresh();
                    });
                });

                await FFMpeg.SetInstalledVersion(latest.PublishedAt);

                File.Delete(tempName);

                AnsiConsole.MarkupLineInterpolated($"[green]FFMpeg version uptated to: {latest.PublishedAt}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[green]FFMpeg is up to date[/]");
            }

            return 0;
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            return -1;
        }
    }
}
