namespace Media.Interop;

internal static class PlaylistFunctions
{
    private static string GetRelativePath(string basePath, string targetPath)
    {
        static string AppendDirectorySeparatorChar(string path)
        {
            if (!Path.EndsInDirectorySeparator(path))
            {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }

        Uri baseUri = new(AppendDirectorySeparatorChar(basePath));
        Uri targetUri = new(targetPath);

        Uri relativeUri = baseUri.MakeRelativeUri(targetUri);
        string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        return relativePath.Replace('/', Path.DirectorySeparatorChar);
    }

    private static async Task WriteM3U(IReadOnlyList<string> playlist,
                                       StreamWriter writer,
                                       bool relativePaths,
                                       string relativeBasePath)
    {
        foreach (var item in playlist)
        {
            if (!relativePaths)
            {
                await writer.WriteLineAsync(item);
            }
            else
            {
                var path = GetRelativePath(relativeBasePath, item);
                await writer.WriteLineAsync(path);
            }
        }
    }

    private static async Task WritePls(IReadOnlyList<string> playlist,
                                       StreamWriter writer,
                                       bool relativePaths,
                                       string relativeBasePath)
    {
        writer.WriteLine("[playlist]");
        for (int i = 0; i < playlist.Count; i++)
        {
            if (!relativePaths)
            {
                await writer.WriteLineAsync($"File{i + 1}={playlist[i]}");
            }
            else
            {
                var path = GetRelativePath(relativeBasePath, playlist[i]);
                await writer.WriteLineAsync($"File{i + 1}={path}");
            }
        }
    }

    public static async Task SaveToFile(this IReadOnlyList<string> playlist, string playlistFile, bool relativePaths)
    {
        var relativeBasePath = Path.GetDirectoryName(playlistFile)
            ?? throw new InvalidOperationException("Couldn't get directory of the file");

        using var writer = File.CreateText(playlistFile);

        var extension = Path.GetExtension(playlistFile).ToLower();

        if (extension == ".m3u" || extension == ".m3u8")
        {
            await WriteM3U(playlist, writer, relativePaths, relativeBasePath);
        }
        else if (extension == ".pls")
        {
            await WritePls(playlist, writer, relativePaths, relativeBasePath);
        }
        throw new InvalidOperationException($"Unknown file type: {extension}");
    }

    private static async Task LoadM3u(IList<string> playlist, string playlistFile)
    {
        using var reader = File.OpenText(playlistFile);
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)
                || line.StartsWith('#'))
            {
                continue;
            }
            playlist.Add(line);
        }
    }

    private static async Task LoadPls(IList<string> playlist, string playlistFile)
    {
        using var reader = File.OpenText(playlistFile);
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line.StartsWith("File"))
            {
                var path = line.Split('=')[1].Trim();
                playlist.Add(path);
            }
        }
    }

    public static async Task LoadFromFile(this IList<string> playlist, string playlistFile)
    {
        var extension = Path.GetExtension(playlistFile).ToLower();

        if (extension == ".m3u" || extension == ".m3u8")
        {
            await LoadM3u(playlist, playlistFile);
        }
        else if (extension == ".pls")
        {
            await LoadPls(playlist, playlistFile);
        }
        throw new InvalidOperationException($"Unknown file type: {extension}");
    }

    public static void Shuffle(this IList<string> playlist)
    {
        int n = playlist.Count;

        for (int i = 0; i < n - 1; i++)
        {
            int j = Random.Shared.Next(i, n);

            if (j != i)
            {
                var temp = playlist[i];
                playlist[i] = playlist[j];
                playlist[j] = temp;
            }
        }
    }

}
