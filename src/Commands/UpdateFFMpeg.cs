// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.IO.Compression;

using Media.Dto.Github;
using Media.Infrastructure;

namespace Media.Commands;

internal sealed class UpdateFFMpeg : BaseGithubUpdateCommand
{
    public UpdateFFMpeg() : base(programName: "FFMpeg",
                                 exeName: "ffmpeg.exe",
                                 repoOwner: "BtbN",
                                 repoName: "FFmpeg-Builds")
    {
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
                        read = await sourceStream.ReadAsync(buffer, 0, buffer.Length);
                        await targetStream.WriteAsync(buffer, 0, read);
                        progress += read;
                        reporter(progress, total);
                    }
                    while (read > 0);
                }
            }
        }
    }

    protected override ReleaseAsset SelectAssetToDownload(ReleaseAsset[] assets)
        => assets.First(a => a.Name.Contains("win64-gpl-shared.zip"));
}