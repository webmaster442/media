using CommunityToolkit.Mvvm.ComponentModel;

namespace Media.Ui;

public class NavigationItem
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

internal sealed class LibNavigatableViewModel : ObservableObject
{
    public BindingList<NavigationItem> Items { get; set; }

    public LibNavigatableViewModel()
    {
        Items = new BindingList<NavigationItem>();
    }
}
