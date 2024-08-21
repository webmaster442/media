namespace Media.Dto;

public sealed class TagData
{
    public required string FileName { get; init; }
    public required string Title { get; init; }
    public required string Artist { get; init; }
    public required string Album { get; init; }
    public required string Genre { get; init; }
    public required uint Year { get; init; }

    public string Comment { get; init; } = string.Empty;
    public string AlbumArtist { get; init; } = string.Empty;
    public string Composer { get; init; } = string.Empty;
    public uint Discnumber { get; init; }
    public uint DiscCount { get; init; }
}
