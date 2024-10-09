// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Internals;
using Media.Infrastructure.Selector;

using Spectre.Console;

namespace Media.Infrastructure.SelectorItemProviders;

internal sealed class FileSystemItemProvider : IItemProvider<Item, string>
{
    private readonly HashSet<string> _extensionsToShow;

    public FileSystemItemProvider(IEnumerable<string>? extensions = null)
    {
        _extensionsToShow = extensions == null
            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
    }

    string IItemProvider<Item, string>.ConvertItem(in Item item)
        => $"{item.Icon} {item.Name}";

    Task<IReadOnlyCollection<Item>> IItemProvider<Item, string>.GetItemsAsync(string currentPath, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Item> items = string.IsNullOrEmpty(currentPath)
            ? GetDrives().ToList()
            : GetFileSystemItems(currentPath);

        return Task.FromResult(items);
    }

    bool IItemProvider<Item, string>.SelectionCanExit(in Item selectedItem)
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
                Name = $"{drive.Name} -{(drive.IsReady ? drive.VolumeLabel : "")}",
                Icon = GetIcon(drive.DriveType)
            };
        }
    }

    private List<Item> GetFileSystemItems(string currentPath)
    {
        static string GetPreviousPath(string currentPath)
        {
            var parts = currentPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(Path.DirectorySeparatorChar, parts.Take(parts.Length - 1));
        }

        List<Item> results = new();

        var directoryInfo = new DirectoryInfo(currentPath);
        try
        {
            results.Add(new Item
            {
                Name = ".. Previous folder",
                FullPath = GetPreviousPath(currentPath),
                Icon = Emoji.Known.FileFolder
            });
            foreach (var directory in directoryInfo.GetDirectories().OrderBy(x => x.Name))
            {
                if (!directory.Attributes.HasFlag(FileAttributes.Hidden)
                    && !directory.Attributes.HasFlag(FileAttributes.System))
                {
                    results.Add(CreateItem(directory));
                }
            }
            foreach (var file in directoryInfo.GetFiles().OrderBy(x => x.Name))
            {
                if (!file.Attributes.HasFlag(FileAttributes.Hidden)
                    && !file.Attributes.HasFlag(FileAttributes.System))
                {
                    if (_extensionsToShow.Count == 0)
                        results.Add(CreateItem(file));
                    else if (_extensionsToShow.Contains(file.Extension))
                        results.Add(CreateItem(file));
                }
            }
        }
        catch (Exception e)
        {
            results.Clear();
            results.Add(new Item
            {
                Name = ".. Previous folder",
                FullPath = GetPreviousPath(currentPath),
                Icon = Emoji.Known.FileFolder
            });
            results.Add(new Item
            {
                Name = e.Message,
                FullPath = "error",
                Icon = Emoji.Known.Warning
            });
        }

        return results;
    }

    public string SelectCurrentPath(in Item item)
        => item.FullPath;
}