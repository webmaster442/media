// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto.MediaDb;

public class VideoFile
{
    public uint Id { get; set; }
    public DateTime AddedDate { get; set; }
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public double PlayTimeInSeconds { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Codecs { get; set; } = string.Empty;
}
