// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto;

namespace Media.Interop;

internal static class TagReader
{
    private readonly static HashSet<string> _extensions 
        = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".mp3",
        ".m4a",
        ".m4b",
        ".mp4",
        ".flac"
    };

    public static IEnumerable<TagData> ReadTags(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            using var tagFile = TagLib.File.Create(file);
            if (tagFile.PossiblyCorrupt) continue;

            yield return new TagData
            {
                Title = tagFile.Tag.Title,
                Album = tagFile.Tag.Album,
                Artist = tagFile.Tag.JoinedPerformers,
                FileName = Path.GetFileName(file),
                Genre = tagFile.Tag.JoinedGenres,
                Year = tagFile.Tag.Year,
                DiscCount = tagFile.Tag.DiscCount,
                Discnumber = tagFile.Tag.Disc,
                AlbumArtist = tagFile.Tag.FirstAlbumArtist,
                Comment = tagFile.Tag.Comment,
                Composer = tagFile.Tag.FirstComposer,
            };

        }
    }
}
