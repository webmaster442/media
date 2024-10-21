// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.IO.Compression;

using Media.Dto.Github;
using Media.Embedded;
using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

[Example("Update media", "media update media")]
internal sealed class UpdateMedia : BaseGithubUpdateCommand
{
    public UpdateMedia()
        : base(programName: "Media CLI",
               exeName: "media.exe",
               repoOwner: "webmaster442",
               repoName: "media")
    {
    }

    protected override async Task ExtractBinariesTo(string compressedFile, string targetPath, Action<long, long> reporter)
    {
        using var zip = new ZipArchive(File.OpenRead(compressedFile));
        ZipArchiveEntry[] entries = zip.Entries.Where(entry => entry.Length > 0).ToArray();
        long total = entries.Sum(entry => entry.Length);
        long progress = 0;
        int read = 0;
        byte[] buffer = new byte[16 * 1024];
        foreach (var entry in entries)
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

    protected override async Task PostInstall(Action<long, long> reporter)
    {
        var targetScriptName = Path.Combine(AppContext.BaseDirectory, "Update.ps1");
        await using (var updateScript = EmbeddedResources.GetFile(EmbeddedResources.UpdatePS1))
        {
            reporter.Invoke(0, 2);
            await using (var targetScript = File.Create(targetScriptName))
            {
                await updateScript.CopyToAsync(targetScript);
                reporter.Invoke(1, 2);
            }
        }

        reporter.Invoke(2, 2);
        using (var process = new Process())
        {
            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = $"-executionpolicy bypass -file {targetScriptName} \"{TargetFolder}\" \"{AppContext.BaseDirectory}\"";
            process.Start();
        }
    }

    protected override string TargetFolder
        => Path.Combine(AppContext.BaseDirectory, "new");

    protected override ReleaseAsset SelectAssetToDownload(ReleaseAsset[] assets)
        => assets.First(a => a.Name.Contains("win.zip"));

    protected override DateTimeOffset? GetInstalledVersion()
    {
        System.Version? version = typeof(UpdateMedia).Assembly?.GetName()?.Version;
        if (version != null)
        {
            return new DateTimeOffset(version.Major, version.Minor, version.Build, 0, 0, 0, TimeSpan.Zero);
        }
        return null;
    }

    protected override Task SetInstalledVersion(DateTimeOffset version)
    {
        return Task.CompletedTask;
    }
}
