namespace Media.Dto.Internals;

internal record class MediaRoute
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string FileUrl { get; init; }
    public required string FilePath { get; init; }
    public required string MimeType { get; init; }
}
