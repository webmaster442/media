namespace Media.Dto.Internals;

internal sealed class GuiCommand
{
    public required string Description { get; init; }
    public required string CommandLine { get; init; }
    public required string ButtonText { get; init; }
    public required GuiCommandPart[] Editors { get; init; }
}
