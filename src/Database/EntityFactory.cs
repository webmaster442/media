using Media.Dto.MediaDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Media.Database;

internal static class EntityFactory
{
    private static readonly TextInfo TextInfo = new CultureInfo("en-US", useUserOverride: false).TextInfo;

    public static async Task<int> CreateEntities(MediaDatabaseContext context, ILogger logger, IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            if (IsMusicFile(file))
            {
                try
                {
                    using TagLib.File f = TagLib.File.Create(file);

                    Album album = await GetOrCreateAlbum(f, context);
                    Genre genre = await GetOrCreateGenre(f, context);

                    context.Music.Add(new MusicFile
                    {
                        Id = CalculateId(file),
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
                try
                {
                    using TagLib.File f = TagLib.File.Create(file);

                    context.Video.Add(new VideoFile
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

    private static async Task<Genre> GetOrCreateGenre(TagLib.File f, MediaDatabaseContext context)
    {
        var genre = f.Tag.FirstGenre.ToTitleCase("Unknown");
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

    private static async Task<Album> GetOrCreateAlbum(TagLib.File f, MediaDatabaseContext context)
    {
        Album toAdd = new Album
        {
            Artist = f.Tag.FirstAlbumArtist.ToTitleCase("Unknown artitst"),
            Name = f.Tag.Album.ToTitleCase("Unknown album"),
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
