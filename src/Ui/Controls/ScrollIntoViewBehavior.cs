using System.Collections.Specialized;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace Media.Ui.Controls;

internal sealed class ScrollIntoViewBehavior : Behavior<ListBox>
{
    protected override void OnAttached()
    {
        ListBox listBox = AssociatedObject;
        ((INotifyCollectionChanged)listBox.Items).CollectionChanged += OnListBox_CollectionChanged;
    }

    protected override void OnDetaching()
    {
        ListBox listBox = AssociatedObject;
        ((INotifyCollectionChanged)listBox.Items).CollectionChanged -= OnListBox_CollectionChanged;
    }

    private void OnListBox_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ListBox listBox = AssociatedObject;
        if (e.Action == NotifyCollectionChangedAction.Add
            && e.NewItems?.Count > 0)
        {
            // scroll the new item into view   
            listBox.ScrollIntoView(e.NewItems[0]);
        }
    }
}
