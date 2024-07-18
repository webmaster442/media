using System.IO.Compression;

namespace FFCmd.Infrastructure;
internal static class FFMpegExtractor
{
    public async static Task ExtractBinariesTo(string zipFile, string targetPath, Action<long, long> reporter)
    {
        using var zip = new ZipArchive(File.OpenRead(zipFile));
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
}
