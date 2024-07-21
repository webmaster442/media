using System.Text.Json;

using FFCmd.Dto.Github;

using Spectre.Console;
using Spectre.Console.Cli;

namespace FFCmd.Infrastructure;

internal abstract class GithubUpdateCommand : AsyncCommand
{
    private readonly string _programName;
    private readonly string _repoOwner;
    private readonly string _repoName;
    private readonly string _updateFileName;

    public GithubUpdateCommand(string programName,
                               string repoOwner,
                               string repoName,
                               string updateFileName)
    {
        _programName = programName;
        _repoOwner = repoOwner;
        _repoName = repoName;
        _updateFileName = updateFileName;
    }

    private static async Task SetInstalledVersion(string versionFileName, DateTimeOffset? publishedAt)
    {
        var versionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, versionFileName);
        using var stream = File.Create(versionFile);
        await JsonSerializer.SerializeAsync(stream, publishedAt);
    }

    private static async Task<DateTimeOffset?> GetInstalledVersion(string versionFileName)
    {
        var versionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, versionFileName);
        if (!File.Exists(versionFile))
        {
            return null;
        }
        using var stream = File.OpenRead(versionFile);
        return await JsonSerializer.DeserializeAsync<DateTimeOffset>(stream);
    }

    private static Release GetLatestRelease(IEnumerable<Release> releases)
    {
        var latest = releases
            .Where(r => !r.Prerelease
                   && !r.Draft
                   && r.Assets.Length > 0)
            .OrderByDescending(r => r.PublishedAt)
            .First();

        return latest;
    }

    protected abstract ReleaseAsset SelectAssetToDownload(ReleaseAsset[] assets);

    protected abstract Task ExtractBinariesTo(string zipFile, string targetPath, Action<long, long> reporter);

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        try
        {
            Terminal.GreenText($"Checking for {_programName} update...");
            using var client = new GithubClient();
            var releases = await client.GetReleases(_repoOwner, _repoName);

            Release latest = GetLatestRelease(releases);

            DateTimeOffset? installed = await GetInstalledVersion(_updateFileName);

            if (installed == null
                || installed < latest.PublishedAt)
            {

                var asset = SelectAssetToDownload(latest.Assets);

                string tempName = string.Empty;

                await AnsiConsole.Progress().AutoRefresh(false).StartAsync(async ctx =>
                {
                    var task1 = ctx.AddTask($"Downloading {_programName.EscapeMarkup()}...");
                    tempName = await client.DownloadAsset(asset, (long position, long length) =>
                    {
                        task1.Value = (double)position / length * 100;
                        ctx.Refresh();
                    });

                    var task2 = ctx.AddTask($"Extracting {_programName.EscapeMarkup()}...");

                    await ExtractBinariesTo(tempName, AppContext.BaseDirectory, (long pogress, long total) =>
                    {
                        task2.Value = (double)pogress / total * 100;
                        ctx.Refresh();
                    });
                });

                await SetInstalledVersion(_updateFileName, latest.PublishedAt);

                File.Delete(tempName);

                Terminal.GreenText($"FFMpeg version uptated to: {latest.PublishedAt}");
            }
            else
            {
                Terminal.GreenText("FFMpeg is up to date");
            }

            return ExitCodes.Success;

        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            return ExitCodes.Exception;
        }
    }
}
