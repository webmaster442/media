// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.Selector;

internal interface IItemProvider<TItem> where TItem : IITem
{
    string ConvertItem(TItem item);

    Task<IReadOnlyCollection<TItem>> GetItemsAsync(string currentPath, CancellationToken cancellationToken);

    bool SelectionCanExit(TItem selectedItem);
}