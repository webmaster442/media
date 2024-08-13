namespace Media.Infrastructure.Selector;

internal sealed class Item : IITem
{
    public required string Name { get; init; }
    public required string FullPath { get; init; }
    public required string Icon { get; init; }
}
