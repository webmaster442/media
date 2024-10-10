// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Threading;

using Media.Dto.Internals;
using Media.Infrastructure.Selector;
using Media.Interop;

using Spectre.Console;

namespace Media.Infrastructure.SelectorItemProviders;

internal sealed class RandomSelectorProvider : IItemProvider<Item, string>
{
    public static IEnumerable<string> ScanSupportedFiles(string parentFolder, bool recursive = true)
    {
        var files = Directory.EnumerateFiles(parentFolder, "*.*", new EnumerationOptions
        {
            RecurseSubdirectories = recursive,
            AttributesToSkip = FileAttributes.System | FileAttributes.Hidden | FileAttributes.Temporary,
            IgnoreInaccessible = true,
        });
        var supported = new HashSet<string>(Mpv.GetSupportedExtensions(), StringComparer.OrdinalIgnoreCase);
        foreach (var file in files)
        {
            var extension = Path.GetExtension(file);
            if (supported.Contains(extension))
            {
                yield return file;
            }
        }
    }

    public static IEnumerable<Item> ScanSubdirectories(string parentFolder)
    {
        var directories = Directory.EnumerateDirectories(parentFolder, "*", new EnumerationOptions
        {
            RecurseSubdirectories = true,
            AttributesToSkip = FileAttributes.System | FileAttributes.Hidden | FileAttributes.Temporary,
            IgnoreInaccessible = true,
        });
        foreach (var directory in directories)
        {
            yield return new Item
            {
                Icon = Emoji.Known.FileFolder,
                Name = Path.GetFileName(directory),
                FullPath = directory
            };
        }
    }

    string IItemProvider<Item, string>.ConvertItem(in Item item)
        => $"{item.Icon} {item.Name}";

    bool IItemProvider<Item, string>.SelectionCanExit(in Item selectedItem)
        => Directory.Exists(selectedItem.FullPath);

    string IItemProvider<Item, string>.SelectCurrentPath(in Item item)
        => item.FullPath;

    Task<IReadOnlyCollection<Item>> IItemProvider<Item, string>.GetItemsAsync(string currentPath, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Item> items = ScanSubdirectories(currentPath).ToList();
        return Task.FromResult(items);
    }
}
