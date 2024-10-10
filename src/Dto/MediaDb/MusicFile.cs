// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto.MediaDb;

public class MusicFile
{
    public uint Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public Album? Album { get; set; }
    public Genre? Genre { get; set; }

    public DateTime AddedDate { get; set; }
    public uint Year { get; set; }
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public double PlayTimeInSeconds { get; set; }
    public uint DiscNumber { get; set; }
    public uint TrackNumber { get; set; }
}
