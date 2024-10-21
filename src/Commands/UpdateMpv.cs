// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Github;
using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

using SharpCompress.Archives.SevenZip;

namespace Media.Commands;

[Example("Update mpv", "media update mpv")]
internal sealed class UpdateMpv : BaseGithubUpdateCommand
{
    private readonly ConfigAccessor _configAccessor;

    public UpdateMpv(ConfigAccessor configAccessor)
        : base(programName: "mpv",
               exeName: "mpv.exe",
               repoOwner: "shinchiro",
               repoName: "mpv-winbuild-cmake")
    {
        _configAccessor = configAccessor;
    }

    protected override async Task ExtractBinariesTo(string compressedFile, string targetPath, Action<long, long> reporter)
    {
        static bool IgnoreEntry(SevenZipArchiveEntry entry)
        {
            return string.IsNullOrEmpty(entry.Key)
                || entry.Size == 0
                || entry.Key.Contains("installer/")
                || entry.Key.Contains("updater.bat");
        }

        using var archive = SevenZipArchive.Open(compressedFile);
        SevenZipArchiveEntry[] entries = archive.Entries.Where(entry => !IgnoreEntry(entry)).ToArray();
        long total = entries.Sum(entry => entry.Size);
        long progress = 0;
        int read = 0;
        byte[] buffer = new byte[16 * 1024];
        foreach (var entry in entries)
        {
            await using (Stream sourceStream = entry.OpenEntryStream())
            {
                var targetName = Path.Combine(targetPath, entry.Key!);
                var targetDir = Path.GetDirectoryName(targetName) ?? throw new InvalidOperationException("Extraction failed");
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                await using (FileStream targetStream = File.Create(targetName))
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

    protected override DateTimeOffset? GetInstalledVersion()
        => _configAccessor.GetMpvVesion();

    protected override ReleaseAsset SelectAssetToDownload(ReleaseAsset[] assets)
        => assets.First(a => a.Name.Contains("mpv-x86_64-"));

    protected override async Task SetInstalledVersion(DateTimeOffset version)
        => await _configAccessor.SetMpvVersion(version);
}