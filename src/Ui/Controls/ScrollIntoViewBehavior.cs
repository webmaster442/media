using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace Media.Ui.Controls;

internal sealed class ScrollIntoViewBehavior : Behavior<ListBox>
{
    protected override void OnAttached()
    {
        ListBox listBox = AssociatedObject;
        listBox.SelectionChanged += OnListBox_SelectionChanged;
    }

    protected override void OnDetaching()
    {
        ListBox listBox = AssociatedObject;
        listBox.SelectionChanged -= OnListBox_SelectionChanged;
    }

    private void OnListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox 
            && e.AddedItems?.Count > 0)
        {
            listBox.ScrollIntoView(e.AddedItems[0]);
        }
    }
}
