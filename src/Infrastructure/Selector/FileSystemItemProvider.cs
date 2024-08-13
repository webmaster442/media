using Spectre.Console;

namespace Media.Infrastructure.Selector;

internal sealed class FileSystemItemProvider : IItemProvider<Item>
{
    private readonly HashSet<string> _extensionsToShow;

    public FileSystemItemProvider(IEnumerable<string>? extensions = null)
    {
        _extensionsToShow = extensions == null
            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
    }

    string IItemProvider<Item>.ConvertItem(Item item)
        => $"{item.Icon} {item.Name}";

    Task<IReadOnlyCollection<Item>> IItemProvider<Item>.GetItemsAsync(string currentPath , CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Item> items = string.IsNullOrEmpty(currentPath)
            ? GetDrives().ToList()
            : (IReadOnlyCollection<Item>)GetFileSystemItems(currentPath).ToList();

        return Task.FromResult(items);
    }

    bool IItemProvider<Item>.SelectionCanExit(Item selectedItem)
        => File.Exists(selectedItem.FullPath);

    private static Item CreateItem(FileSystemInfo item)
    {
        return new Item
        {
            FullPath = item.FullName,
            Name = item.Name,
            Icon = item is DirectoryInfo ? Emoji.Known.FileFolder : Emoji.Known.Memo
        };
    }

    private static IEnumerable<Item> GetDrives()
    {
        static string GetIcon(DriveType driveType)
        {
            return driveType switch
            {
                DriveType.Fixed => Emoji.Known.ComputerDisk,
                DriveType.Ram => Emoji.Known.DesktopComputer,
                DriveType.Removable => Emoji.Known.FloppyDisk,
                DriveType.CDRom => Emoji.Known.OpticalDisk,
                DriveType.NoRootDirectory => Emoji.Known.FileFolder,
                DriveType.Network => Emoji.Known.GlobeWithMeridians,
                _ => Emoji.Known.WhiteQuestionMark,
            };
        }

        foreach (var drive in DriveInfo.GetDrives())
        {
            yield return new Item
            {
                FullPath = drive.RootDirectory.FullName,
                Name = $"{drive.Name} - {drive.VolumeLabel}",
                Icon = GetIcon(drive.DriveType)
            };
        }
    }

    private static string GetPreviousPath(string currentPath)
    {
        var parts = currentPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(Path.DirectorySeparatorChar, parts.Take(parts.Length - 1));
    }

    private IEnumerable<Item> GetFileSystemItems(string currentPath)
    {
        var directoryInfo = new DirectoryInfo(currentPath);
        yield return new Item
        {
            Name = "..",
            FullPath = GetPreviousPath(currentPath),
            Icon = ""
        };
        foreach (var directory in directoryInfo.GetDirectories().OrderBy(x => x.Name))
        {
            if (!directory.Attributes.HasFlag(FileAttributes.Hidden)
                && !directory.Attributes.HasFlag(FileAttributes.System))
            {
                yield return CreateItem(directory);
            }
        }
        foreach (var file in directoryInfo.GetFiles().OrderBy(x => x.Name))
        {
            if (!file.Attributes.HasFlag(FileAttributes.Hidden)
                && !file.Attributes.HasFlag(FileAttributes.System))
            {
                if (_extensionsToShow.Count == 0)
                    yield return CreateItem(file);
                else if (_extensionsToShow.Contains(file.Extension))
                    yield return CreateItem(file);
            }
        }
    }
}