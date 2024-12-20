// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Database.Entity;

public sealed class Album
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public uint Year { get; set; }
    public ICollection<MusicFile> Tracks { get; set; } = new HashSet<MusicFile>();
}