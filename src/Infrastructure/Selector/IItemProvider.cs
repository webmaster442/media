namespace Media.Infrastructure.Selector;

internal interface IItemProvider<TItem> where TItem : IITem
{
    string ConvertItem(TItem item);

    Task<IReadOnlyCollection<TItem>> GetItemsAsync(string currentPath, CancellationToken cancellationToken);

    bool SelectionCanExit(TItem selectedItem);
}