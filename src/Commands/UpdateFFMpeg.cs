// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.IO.Compression;

using Media.Dto.Github;
using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

[Example("Update ffmpeg", "media update ffmpeg")]
internal sealed class UpdateFFMpeg : BaseGithubUpdateCommand
{
    private readonly ConfigAccessor _configAccessor;

    public UpdateFFMpeg(ConfigAccessor configAccessor)
        : base(programName: "FFMpeg",
               exeName: "ffmpeg.exe",
               repoOwner: "BtbN",
               repoName: "FFmpeg-Builds")
    {
        _configAccessor = configAccessor;
    }

    protected override async Task ExtractBinariesTo(string compressedFile, string targetPath, Action<long, long> reporter)
    {
        using var zip = new ZipArchive(File.OpenRead(compressedFile));
        ZipArchiveEntry[] binFolder = zip.Entries.Where(entry => entry.FullName.Contains("/bin/") && entry.Length > 0).ToArray();
        long total = binFolder.Sum(entry => entry.Length);
        long progress = 0;
        int read = 0;
        byte[] buffer = new byte[16 * 1024];
        foreach (var entry in binFolder)
        {
            await using (Stream sourceStream = entry.Open())
            {
                await using (FileStream targetStream = File.Create(Path.Combine(targetPath, entry.Name)))
                {
                    do
                    {
                        read = await sourceStream.ReadAsync(buffer);
                        await targetStream.WriteAsync(buffer, 0, read);
                        progress += read;
                        reporter(progress, total);
                    }
                    while (read > 0);
                }
            }
        }
    }

    protected override DateTimeOffset? GetInstalledVersion()
        => _configAccessor.GetFFMPegVesion();

    protected override ReleaseAsset SelectAssetToDownload(ReleaseAsset[] assets)
        => assets.First(a => a.Name.EndsWith("-win64-gpl-shared.zip"));

    protected override async Task SetInstalledVersion(DateTimeOffset version)
        => await _configAccessor.SetFFMpegVersion(version);
}