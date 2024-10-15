using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Media.Ui;

internal sealed class LibNavigatableViewModel : ObservableObject
{
    public ObservableCollection<NavigationItem> VisibleItems { get; }

    public LibNavigatableViewModel(IEnumerable<NavigationItem> items)
    {
        VisibleItems = new ObservableCollection<NavigationItem>(items);
    }
}
