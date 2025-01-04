namespace Media.Ui.Gui;

internal sealed class BookmarkViewModel
{
    public required string Path { get; init; }
    public required string Name { get; init; }

    public bool IsEnabled => Directory.Exists(Path);
}
