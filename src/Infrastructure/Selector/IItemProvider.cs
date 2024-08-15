// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.Selector;

internal interface IItemProvider<TItem, TPath> where TItem : IITem
{
    string ConvertItem(in TItem item);
    Task<IReadOnlyCollection<TItem>> GetItemsAsync(TPath currentPath, CancellationToken cancellationToken);
    TPath SelectCurrentPath(in TItem item);
    bool SelectionCanExit(in TItem selectedItem);
}