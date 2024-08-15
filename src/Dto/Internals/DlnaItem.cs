using Media.Infrastructure.Selector;

namespace Media.Dto.Internals;

public sealed record class DlnaItem : IITem
{
    public required string Name { get; init; }
    public required Uri Uri { get; init; }
    public bool IsServer { get; init; }
    public bool IsBrowsable { get; init; }
    public required string Id { get; init; }

    public string FullPath => Uri.ToString();
}
