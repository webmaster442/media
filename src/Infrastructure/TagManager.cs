using Media.Dto;

using TagLib;

namespace Media.Infrastructure;

public static class TagManager
{
    public static IEnumerable<TagData> ReadTags(string[] files)
    {
        foreach (var file in files)
        {
            using (var tagFile = TagLib.File.Create(file))
            {
                Tag tag = tagFile.Tag;

                yield return new TagData
                {
                    FileName = Path.GetFileName(file),
                    Title = tag.Title,
                    Artist = tag.Performers.FirstOrDefault(string.Empty),
                    Album = tag.Album,
                    Genre = tag.FirstGenre,
                    Year = tag.Year,
                    Comment = tag.Comment,
                    AlbumArtist = tag.FirstAlbumArtist,
                    Composer = tag.FirstComposer,
                    Discnumber = tag.Disc,
                    DiscCount = tag.DiscCount
                };
            }
        }
    }
}
