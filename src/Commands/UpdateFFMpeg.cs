using System.IO.Compression;

using FFCmd.Dto.Github;
using FFCmd.Infrastructure;

namespace FFCmd.Commands;

internal sealed class UpdateFFMpeg : GithubUpdateCommand
{
    public UpdateFFMpeg() : base(programName: "FFMpeg",
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
            using (Stream sourceStream = entry.Open())
            {
                using (FileStream targetStream = File.Create(Path.Combine(targetPath, entry.Name)))
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
