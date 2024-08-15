namespace Media.Dto.Internals;

public sealed record class DlnaItem
{
    public required string Name { get; init; }
    public required string FullPath { get; init; }
    public bool IsServer { get; init; }
    public bool IsBrowsable { get; init; }
    public required string Id { get; init; }
}
