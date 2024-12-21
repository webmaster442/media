// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Database.Entity;

public class PlayedEntry
{
    public required string Path { get; set; }
    public required DateTime LastPlayed { get; set; }
}
