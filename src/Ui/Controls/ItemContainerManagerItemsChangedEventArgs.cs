using System.Collections.Specialized;


namespace Media.Ui.Controls;

internal class ItemContainerManagerItemsChangedEventArgs
{
    public NotifyCollectionChangedAction Action { get; }

    public ItemContainerManagerItemsChangedEventArgs(NotifyCollectionChangedAction action)
    {
        Action = action;
    }
}
