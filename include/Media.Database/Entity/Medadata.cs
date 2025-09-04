namespace Media.Database.Entity;

public class Metadata
{
    public required string Path { get; set; }
    public required string Title { get; set; }
    public required string Artist { get; set; }
    public required string Album { get; set; }
    public required string Genre { get; set; }
    public required uint Year { get; set; }
    public required long Size { get; set; }
    public required uint DiscNumber { get; set; }
    public required uint TrackNumber { get; set; }
    public required double PlayTimeInSeconds { get; set; }
    public required string Codecs { get; set; }
    public int VideoWidth { get; set; }
    public int VideoHeight { get; set; }
}
