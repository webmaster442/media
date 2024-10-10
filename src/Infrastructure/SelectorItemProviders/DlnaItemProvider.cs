// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Threading;

using Media.Dto.Internals;
using Media.Infrastructure.Dlna;
using Media.Infrastructure.Selector;

using Spectre.Console;

namespace Media.Infrastructure.SelectorItemProviders;

public sealed class DlnaItemProvider : IItemProvider<DlnaItem, DlnaItemProvider.CurrentPath>, IDisposable
{
    public sealed class CurrentPath
    {
        internal static readonly CurrentPath Empty = new()
        {
            Id = string.Empty,
            Name = string.Empty,
            Uri = string.Empty
        };

        public required string Uri { get; init; }
        public required string Id { get; init; }
        public required string Name { get; init; }

        public override string ToString()
        {
            return Name;
        }
    }

    private readonly DLNAClient _client;

    public DlnaItemProvider()
    {
        _client = new DLNAClient();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    string IItemProvider<DlnaItem, CurrentPath>.ConvertItem(in DlnaItem item)
    {
        if (item.IsServer)
            return $"{Emoji.Known.DesktopComputer}  {item.Name}";
        else if (item.IsBrowsable)
            return $"{Emoji.Known.FileFolder} {item.Name}";
        else
            return $"{Emoji.Known.Memo} {item.Name}";
    }

    async Task<IReadOnlyCollection<DlnaItem>> IItemProvider<DlnaItem, CurrentPath>.GetItemsAsync(CurrentPath currentPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentPath.Uri))
        {
            var servers = await _client.GetServersAsync(cancellationToken);
            if (servers.Count < 1)
            {
                return new DlnaItem[]
                {
                    new() {
                        Name = "No servers found",
                        IsBrowsable = false,
                        IsServer = true,
                        Uri = new Uri("none://"),
                        Id = "0"
                    }
                };
            }
            return servers;
        }

        return await _client.Browse(currentPath.Uri, currentPath.Id, cancellationToken);
    }

    CurrentPath IItemProvider<DlnaItem, CurrentPath>.SelectCurrentPath(in DlnaItem item)
        => new CurrentPath
        {
            Uri = item.Uri.ToString(),
            Id = string.IsNullOrEmpty(item.Id) ? "0" : item.Id,
            Name = item.Name
        };

    bool IItemProvider<DlnaItem, CurrentPath>.SelectionCanExit(in DlnaItem selectedItem)
        => !selectedItem.IsBrowsable;
}
