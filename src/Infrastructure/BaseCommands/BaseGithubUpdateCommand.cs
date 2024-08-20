// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Github;

using Spectre.Console;

namespace Media.Infrastructure;

internal abstract class BaseGithubUpdateCommand : AsyncCommand
{
    private readonly string _exeName;
    private readonly string _programName;
    private readonly string _repoOwner;
    private readonly string _repoName;

    private readonly ConfigAccessor _configAccessor;

    protected BaseGithubUpdateCommand(string programName,
                                      string exeName,
                                      string repoOwner,
                                      string repoName)
    {
        _exeName = exeName;
        _programName = programName;
        _repoOwner = repoOwner;
        _repoName = repoName;
        _configAccessor = new ConfigAccessor();
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

    protected abstract Task ExtractBinariesTo(string compressedFile, string targetPath, Action<long, long> reporter);

    protected virtual string TargetFolder { get; } = AppContext.BaseDirectory;

    protected virtual Task PostInstall(Action<long, long> reporter)
    {
        reporter.Invoke(1, 1);
        return Task.CompletedTask;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        try
        {
            Terminal.GreenText($"Checking for {_programName} update...");
            using var client = new GithubClient();
            var releases = await client.GetReleases(_repoOwner, _repoName);

            Release latest = GetLatestRelease(releases);

            DateTimeOffset? installed = _configAccessor.GetInstalledVersion(_programName);

            if (installed == null
                || installed < latest.PublishedAt
                || !File.Exists(_exeName))
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

                    await ExtractBinariesTo(tempName, TargetFolder, (long pogress, long total) =>
                    {
                        task2.Value = (double)pogress / total * 100;
                        ctx.Refresh();
                    });

                    var postExtract = ctx.AddTask($"Post install actions...");
                    await PostInstall((long pogress, long total) =>
                    {
                        postExtract.Value = (double)pogress / total * 100;
                        ctx.Refresh();
                    });

                });

                await _configAccessor.SetInstalledVersion(_programName, latest.PublishedAt);



                File.Delete(tempName);

                Terminal.GreenText($"{_programName} uptated to: {latest.PublishedAt}");
            }
            else
            {
                Terminal.GreenText($"{_programName} is up to date");
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
