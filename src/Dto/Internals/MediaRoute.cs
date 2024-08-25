namespace Media.Dto.Internals;

internal record class MediaRoute
{
    public required string FileUrl { get; init; }
    public required string FilePath { get; init; }
    public required string MimeType { get; init; }
}
