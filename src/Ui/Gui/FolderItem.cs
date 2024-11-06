using Media.Interop;

namespace Media.Ui.Gui;

internal class FolderItem
{
    public string Name => System.IO.Path.GetFileName(FullPath);
    public required string FullPath { get; init; }
    public required DateTime LastModified { get; init; }
    public required string Size { get; init; }
    public required bool IsDirectory { get; init; }
    public required FileRecognizer.FileType FileType { get; init; }
    public required string Extension { get; init; }
}
