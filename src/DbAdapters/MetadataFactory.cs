using System.Diagnostics.CodeAnalysis;

using Media.Database.Entity;

namespace Media.DbAdapters;

internal static class MetadataFactory
{
    private static readonly TextInfo _textInfo
        = new CultureInfo("en-US", false).TextInfo;

    public static bool TryCreateMetaData(string path, [MaybeNullWhen(false)] out Metadata metadata)
    {
        try
        {
            using TagLib.File f = TagLib.File.Create(path);
            metadata = new Metadata
            {
                Path = path,
                Artist = ToTitleCase(f.Tag.FirstPerformer, "Unknown artitst"),
                Title = ToTitleCase(f.Tag.Title, Path.GetFileNameWithoutExtension(path)),
                Size = f.Length,
                Year = f.Tag.Year,
                TrackNumber = f.Tag.Track,
                DiscNumber = f.Tag.Disc,
                PlayTimeInSeconds = f.Properties.Duration.TotalSeconds,
                Album = ToTitleCase(f.Tag.Album, "Unknown album"),
                Genre = ToTitleCase(f.Tag.FirstGenre, "Unknown genre"),
                Codecs = string.Join(',', f.Properties.Codecs.Select(x => x.Description)),
                VideoWidth = f.Properties.VideoWidth,
                VideoHeight = f.Properties.VideoHeight,
            };
            return true;
        }
        catch (Exception)
        {
            metadata = null;
            return false;
        }
    }

    private static string ToTitleCase(string s, string onEmptyValue)
    {
        if (string.IsNullOrEmpty(s))
            return _textInfo.ToTitleCase(onEmptyValue);

        return _textInfo.ToTitleCase(s.ToLower());
    }
}