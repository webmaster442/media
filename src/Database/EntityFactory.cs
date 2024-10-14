// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.MediaDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Media.Database;

internal static class EntityFactory
{
    private static readonly TextInfo TextInfo = new CultureInfo("en-US", useUserOverride: false).TextInfo;

    public static async Task<int> CreateEntities(MediaDatabaseContext context, ILogger logger, IEnumerable<string> files)
    {
        Dictionary<uint, Album> albumCache = new();
        Dictionary<uint, Genre> genreCache = new();

        foreach (var file in files)
        {
            uint id = CalculateId(file);
            if (IsMusicFile(file))
            {
                if (context.Music.AsNoTracking().Where(m => m.Id == id).Any())
                {
                    logger.LogInformation("Skipping {file}, allready in db", file);
                    continue;
                }

                try
                {
                    using TagLib.File f = TagLib.File.Create(file);

                    Album album = await GetOrCreateAlbum(f, context, albumCache);
                    Genre genre = await GetOrCreateGenre(f, context, genreCache);

                    context.Music.Add(new MusicFile
                    {
                        Id = id,
                        AddedDate = DateTime.UtcNow,
                        Artist = f.Tag.FirstPerformer.ToTitleCase("Unknown artitst"),
                        Title = f.Tag.Title.ToTitleCase(Path.GetFileNameWithoutExtension(file)),
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
            else if (IsVideoFile(file))
            {
                if (context.Video.AsNoTracking().Where(v => v.Id == id).Any())
                {
                    logger.LogInformation("Skipping {file}, allready in db", file);
                    continue;
                }

                try
                {
                    using TagLib.File f = TagLib.File.Create(file);

                    context.Video.Add(new VideoFile
                    {
                        Id = id,
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
            else
            {
                logger.LogInformation("Skipping {file}", file);
            }
        }
        return await context.SaveChangesAsync();
    }

    private static async Task<Genre> GetOrCreateGenre(TagLib.File f,
                                                      MediaDatabaseContext context,
                                                      Dictionary<uint, Genre> genreCache)
    {
        var genre = f.Tag.FirstGenre.ToTitleCase("Unknown");
        var id = CalculateId(genre);

        if (genreCache.ContainsKey(id))
        {
            return genreCache[id];
        }

        Genre? found = await context.Genres.FirstOrDefaultAsync(g => g.Id == id);
        if (found != null)
        {
            genreCache.Add(id, found);
            return found;
        }
        else
        {
            var g = new Genre
            {
                Id = id,
                Name = genre,
            };
            genreCache.Add(id, g);
            return g;
        }
    }

    private static async Task<Album> GetOrCreateAlbum(TagLib.File f, MediaDatabaseContext context, Dictionary<uint, Album> albumCache)
    {
        var artist = f.Tag.FirstAlbumArtist.ToTitleCase("Unknown artitst");
        var name = f.Tag.Album.ToTitleCase("Unknown album");
        var year = f.Tag.Year;
        var id = CalculateId($"{artist} - {name} - {year}");

        if (albumCache.ContainsKey(id))
        {
            return albumCache[id];
        }

        Album? found = await context.Albums.FirstOrDefaultAsync(a => a.Id == id);

        if (found != null)
        {
            albumCache.Add(id, found);
            return found;
        }
        else
        {
            var a = new Album
            {
                Id = id,
                Artist = artist,
                Name = name,
                Year = year,
            };
            albumCache.Add(id, a);
            return a;
        }
    }

    private static bool IsMusicFile(string file)
    {
        var extension = Path.GetExtension(file).ToLower();
        return extension == ".mp3"
            || extension == ".m4b"
            || extension == ".m4a"
            || extension == ".ogg"
            || extension == ".wma"
            || extension == ".flac"
            || extension == ".wav"
            || extension == ".weba"
            || extension == "oga";
    }

    private static bool IsVideoFile(string file)
    {
        var extension = Path.GetExtension(file).ToLower();
        return extension == ".avi"
            || extension == ".flv"
            || extension == ".mkv"
            || extension == ".mov"
            || extension == ".m4v"
            || extension == ".mp4"
            || extension == ".mpg"
            || extension == ".mpeg"
            || extension == ".ts"
            || extension == ".m2ts"
            || extension == ".mts";
    }

    private static string ToTitleCase(this string s, string onEmptyValue)
    {
        if (string.IsNullOrEmpty(s))
            return TextInfo.ToTitleCase(onEmptyValue);

        return TextInfo.ToTitleCase(s.ToLower());
    }

    private static uint CalculateId(this string s)
    {
        uint hash = 2166136261u;
        foreach (var chr in s)
        {
            hash ^= chr;
            hash *= 16777619u;
        }
        return hash;
    }
}