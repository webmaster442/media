// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Media.Ui.Controls;

internal sealed class ObservableRangeCollection<T> : ObservableCollection<T>
{
    private bool AddArrangeCore(IEnumerable<T> collection)
    {
        var itemAdded = false;
        foreach (var item in collection)
        {
            Items.Add(item);
            itemAdded = true;
        }
        return itemAdded;
    }

    private void RaiseChangeNotificationEvents(NotifyCollectionChangedAction action, List<T>? changedItems = null, int startingIndex = -1)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

        if (changedItems is null)
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
        else
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems: changedItems, startingIndex: startingIndex));
    }

    public void AddRange(IEnumerable<T> collection, bool raisesAddEventInsteadOfReset = false)
    {
        ArgumentNullException.ThrowIfNull(collection);

        var notificationMode = raisesAddEventInsteadOfReset
            ? NotifyCollectionChangedAction.Add
            : NotifyCollectionChangedAction.Reset;

        CheckReentrancy();

        int startIndex = Count;

        bool itemsAdded = AddArrangeCore(collection);

        if (!itemsAdded)
            return;

        if (notificationMode == NotifyCollectionChangedAction.Reset)
        {
            RaiseChangeNotificationEvents(action: NotifyCollectionChangedAction.Reset);
            return;
        }

        List<T> changedItems = collection is List<T>
            ? (List<T>)collection
            : new List<T>(collection);

        RaiseChangeNotificationEvents(action: NotifyCollectionChangedAction.Add,
                                      changedItems: changedItems,
                                      startingIndex: startIndex);
    }
}
