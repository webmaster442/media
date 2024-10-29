namespace Media.Dto.Internals;

internal class GuiCommandPart
{
    public required string Name { get; init; }
    public required GuiCommandPartEditor Editor { get; init; }
    public required string Description { get; init; }
}
