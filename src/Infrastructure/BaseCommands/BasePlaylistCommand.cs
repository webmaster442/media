// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.BaseCommands;

internal abstract class BasePlaylistCommand<T> : BaseFileWorkCommand<T> where T : ValidatedCommandSettings
{
    protected async Task<Playlist> LoadFromFile(string playlistFile)
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

    protected async Task SaveToFile(Playlist playlist, string playlistFile, bool relativePaths)
    {
        var relativeBasePath = Path.GetDirectoryName(playlistFile) 
            ?? throw new InvalidOperationException("Couldn't get directory of the file");

        using var writer = File.CreateText(playlistFile);

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
}
