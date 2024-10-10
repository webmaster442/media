// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto.MediaDb;

public class Genre
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<MusicFile> Tracks { get; set; } = new HashSet<MusicFile>();
}
