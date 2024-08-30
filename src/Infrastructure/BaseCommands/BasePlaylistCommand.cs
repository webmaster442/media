// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.BaseCommands;

internal abstract class BasePlaylistCommand<T> : BaseFileWorkCommand<T> where T : ValidatedCommandSettings
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

    private static async Task<Playlist> LoadPls(string playlistFile)
    {
        using var reader = File.OpenText(playlistFile);
        var playlist = new Playlist();

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line.StartsWith("File"))
            {
                var path = line.Split('=')[1].Trim();
                playlist.Add(path);
            }
        }

        return playlist;
    }

    private static async Task WritePls(Playlist playlist,
                                       StreamWriter writer,
                                       bool relativePaths,
                                       string relativeBasePath)
    {
        writer.WriteLine("[playlist]");
        for (int i=0; i<playlist.Count; i++)
        {
            if (!relativePaths)
            {
                await writer.WriteLineAsync($"File{i+1}={playlist[i]}");
            }
            else
            {
                var path = GetRelativePath(relativeBasePath, playlist[i]);
                await writer.WriteLineAsync($"File{i+1}={path}");
            }
        }
    }

    private static async Task<Playlist> LoadM3u(string playlistFile)
    {
        using var reader = File.OpenText(playlistFile);
        var playlist = new Playlist();

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

        return playlist;
    }

    private static async Task WriteM3U(Playlist playlist,
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

    protected async Task<Playlist> LoadFromFile(string playlistFile)
    {
        var extension = Path.GetExtension(playlistFile).ToLower();

        if (extension == ".m3u" || extension == ".m3u8")
        {
            return await BasePlaylistCommand<T>.LoadM3u(playlistFile);
        }
        else if (extension == ".pls")
        {
            return await BasePlaylistCommand<T>.LoadPls(playlistFile);
        }
        throw new InvalidOperationException($"Unknown file type: {extension}");
    }

    protected async Task SaveToFile(Playlist playlist, string playlistFile, bool relativePaths)
    {
        var relativeBasePath = Path.GetDirectoryName(playlistFile)
            ?? throw new InvalidOperationException("Couldn't get directory of the file");

        using var writer = File.CreateText(playlistFile);

        var extension = Path.GetExtension(playlistFile).ToLower();

        if (extension == ".m3u" || extension == ".m3u8")
        {
            await BasePlaylistCommand<T>.WriteM3U(playlist, writer, relativePaths, relativeBasePath);
        }
        else if (extension == ".pls")
        {
            await BasePlaylistCommand<T>.WritePls(playlist, writer, relativePaths, relativeBasePath);
        }
        throw new InvalidOperationException($"Unknown file type: {extension}");
    }
}
