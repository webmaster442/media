

using Media.Dto.Internals;

namespace Media.Infrastructure.Selector;

public sealed class DlnaItemProvider : IItemProvider<DlnaItem>, IDisposable
{
    private readonly DLNAClient _client;

    public DlnaItemProvider()
    {
        _client = new DLNAClient();
    }

    public void Dispose()
        => _client.Dispose();

    public string ConvertItem(DlnaItem item)
    {
        return item.Name;
    }

    public async Task<IReadOnlyCollection<DlnaItem>> GetItemsAsync(string currentPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentPath))
            return await _client.GetServersAsync(cancellationToken);
    }

    public bool SelectionCanExit(DlnaItem selectedItem)
    {
        return !selectedItem.IsBrowsable
            && !selectedItem.IsServer;
    }

    public string ModifyCurrentPath(DlnaItem item)
    {
        throw new NotImplementedException();
    }
}
