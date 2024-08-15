﻿

using Media.Dto.Internals;

namespace Media.Infrastructure.Selector;

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
        => item.Name;

    Task<IReadOnlyCollection<DlnaItem>> IItemProvider<DlnaItem, CurrentPath>.GetItemsAsync(CurrentPath currentPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentPath.Id))
            return _client.GetServersAsync(cancellationToken);

        throw new NotImplementedException();
    }

    CurrentPath IItemProvider<DlnaItem, CurrentPath>.SelectCurrentPath(in DlnaItem item)
        => new CurrentPath
        {
            Uri = item.Uri.ToString(),
            Id = item.Id,
            Name = item.Name
        };

    bool IItemProvider<DlnaItem, CurrentPath>.SelectionCanExit(in DlnaItem selectedItem)
        => !selectedItem.IsBrowsable;
}
