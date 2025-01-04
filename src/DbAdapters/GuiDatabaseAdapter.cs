// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;
using Media.Database.Entity;
using Media.Interop;
using Media.Ui.Controls;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Media.DbAdapters;

internal class GuiDatabaseAdapter : DatabaseAdapterBase
{
    private readonly TextInfo _textInfo;

    public GuiDatabaseAdapter()
    {
        _textInfo = new CultureInfo("en-US", false).TextInfo;
    }

    public async Task<List<PlayedEntry>> GetPlayedEntries(DateTime start, DateTime end)
    {
        using var dbContext = GetContext();
        return await dbContext.PlayedEntries
            .Where(p => p.LastPlayed <= end)
            .Where(p => p.LastPlayed >= start)
            .ToListAsync();
    }

    public async Task<List<FolderBookmark>> GetBookmarks()
    {
        using var dbContext = GetContext();
        return await dbContext.FolderBookmarks.ToListAsync();
    }

    public async Task AddBookmark(string path)
    {
        using var dbContext = GetContext();
        var entry = dbContext.FolderBookmarks.FirstOrDefault(e => e.Path == path);
        if (entry == null)
        {
            dbContext.FolderBookmarks.Add(new FolderBookmark
            {
                Name = Path.GetFileName(path),
                Path = path,
            });
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<int> AddToLibary(IEnumerable<string> files, ILogger logger)
    {
        using var context = GetContext();
        foreach (var file in files)
        {
            var fileType = FileRecognizer.GetFileType(file);
            if (fileType == FileRecognizer.FileType.Audio)
            {
                try
                {
                    using TagLib.File f = TagLib.File.Create(file);
                    Album album = await GetOrCreateAlbum(f, context);
                    Genre genre = await GetOrCreateGenre(f, context);
                    context.Musics.Add(new MusicFile
                    {
                        Id = CalculateId(file),
                        AddedDate = DateTime.UtcNow,
                        Artist = ToTitleCase(f.Tag.FirstPerformer, "Unknown artitst"),
                        Title = ToTitleCase(f.Tag.Title, Path.GetFileNameWithoutExtension(file)),
                        Size = f.Length,
                        Year = f.Tag.Year,
                        TrackNumber = f.Tag.Track,
                        DiscNumber = f.Tag.Disc,
                        PlayTimeInSeconds = f.Properties.Duration.TotalSeconds,
                        Path = file,
                        Album = album,
                        Genre = genre,
                    });
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Music file read error: {ex}", ex);
                }
            }
            else if (fileType == FileRecognizer.FileType.Video)
            {
                try
                {
                    using TagLib.File f = TagLib.File.Create(file);
                    context.Videos.Add(new VideoFile
                    {
                        Id = CalculateId(file),
                        Path = file,
                        AddedDate = DateTime.UtcNow,
                        Size = f.Length,
                        PlayTimeInSeconds = f.Properties.Duration.TotalSeconds,
                        Codecs = string.Join(',', f.Properties.Codecs.Select(x => x.Description)),
                        Width = f.Properties.VideoWidth,
                        Height = f.Properties.VideoHeight,
                    });
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Video file read error: {ex}", ex);
                }
            }
        }
        return await context.SaveChangesAsync();
    }

    private async Task<Genre> GetOrCreateGenre(TagLib.File f, DatabaseContext context)
    {
        var genre = ToTitleCase(f.Tag.FirstGenre, "Unknown");
        var toAdd = new Genre
        {
            Id = CalculateId(genre),
            Name = genre,
        };
        Genre? found = await context.Genres.FirstOrDefaultAsync(g => g.Id == toAdd.Id);
        if (found != null)
        {
            return found;
        }
        context.Genres.Add(toAdd);
        return toAdd;
    }

    private async Task<Album> GetOrCreateAlbum(TagLib.File f, DatabaseContext context)
    {
        Album toAdd = new Album
        {
            Artist = ToTitleCase(f.Tag.FirstAlbumArtist, "Unknown artitst"),
            Name = ToTitleCase(f.Tag.Album, "Unknown album"),
            Year = f.Tag.Year,
        };
        toAdd.Id = CalculateId($"{toAdd.Artist} - {toAdd.Id}");
        Album? found = await context.Albums.FirstOrDefaultAsync(a => a.Id == toAdd.Id);
        if (found != null)
        {
            return found;
        }
        context.Albums.Add(toAdd);
        return toAdd;
    }

    private static uint CalculateId(string s)
    {
        uint hash = 2166136261u;
        foreach (var chr in s)
        {
            hash ^= chr;
            hash *= 16777619u;
        }
        return hash;
    }

    private string ToTitleCase(string s, string onEmptyValue)
    {
        if (string.IsNullOrEmpty(s))
            return _textInfo.ToTitleCase(onEmptyValue);

        return _textInfo.ToTitleCase(s.ToLower());
    }
}
