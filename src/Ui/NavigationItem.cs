namespace Media.Ui;

public sealed class NavigationItem
{
    public required string Title { get; set; } = string.Empty;
    public required string Url { get; set; } = string.Empty;

    public const string AlbumPrefix = "album";
    public const string ArtistPrefix = "artist";
    public const string GenrePrefix = "genre";
    public const string YearPrefix = "year";

}
