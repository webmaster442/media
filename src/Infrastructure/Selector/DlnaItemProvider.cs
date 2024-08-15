

using Media.Dto.Internals;

namespace Media.Infrastructure.Selector;

public sealed class DlnaItemProvider : IItemProvider<DlnaItem, DlnaItemProvider.CurrentPath>, IDisposable
{
    public record class CurrentPath(string Uri, string Id);

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
        throw new NotImplementedException();
    }

    CurrentPath IItemProvider<DlnaItem, CurrentPath>.SelectCurrentPath(in DlnaItem item)
        => new CurrentPath(item.Uri.ToString(), item.Id);

    bool IItemProvider<DlnaItem, CurrentPath>.SelectionCanExit(in DlnaItem selectedItem)
        => !selectedItem.IsBrowsable;
}
