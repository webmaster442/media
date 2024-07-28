namespace Media.Dto.Internals;

internal sealed record class YtDlpFormat
{
    public required string Id { get; init; }
    public required string Format { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required int BitrateInK { get; init; }
    public required string Codec { get; init; }
}
