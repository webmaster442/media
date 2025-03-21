// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.BaseCommands;
using Media.DbAdapters;
using Media.Dto.Github;
using Media.Infrastructure;

namespace Media.Commands.Update;

[Example("Update yt-dlp", "media update ytdlp")]
internal class UpdateYtdlp : BaseGithubUpdateCommand
{
    private readonly ConfigAdapter _configAccessor;

    public UpdateYtdlp(ConfigAdapter configAccessor)
        : base(programName: "yt-dlp",
               exeName: "yt-dlp.exe",
               repoOwner: "yt-dlp",
               repoName: "yt-dlp")
    {
        _configAccessor = configAccessor;
    }

    protected override async Task ExtractBinariesTo(string compressedFile, string targetPath, Action<long, long> reporter)
    {
        await using var sourceStream = File.OpenRead(compressedFile);
        await using var targetStream = File.Create(Path.Combine(targetPath, "yt-dlp.exe"));
        long progress = 0;
        int read = 0;
        byte[] buffer = new byte[16 * 1024];
        long total = sourceStream.Length;
        do
        {
            read = await sourceStream.ReadAsync(buffer);
            await targetStream.WriteAsync(buffer, 0, read);
            progress += read;
            reporter(progress, total);
        }
        while (read > 0);
    }

    protected override DateTimeOffset? GetInstalledVersion()
        => _configAccessor.YtdlpVesion;

    protected override ReleaseAsset SelectAssetToDownload(ReleaseAsset[] assets)
        => assets.First(a => a.Name.Contains("yt-dlp.exe"));

    protected override void SetInstalledVersion(DateTimeOffset version)
    {
        _configAccessor.YtdlpVesion = version;
    }
}