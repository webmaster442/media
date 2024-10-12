using CommunityToolkit.Mvvm.ComponentModel;

using Media.Interfaces;

namespace Media.Ui;

internal sealed partial class LibWindowViewModel : ObservableObject, IViewModel
{
    [ObservableProperty]
    private object _contentItem;

    public LibWindowViewModel()
    {
        _contentItem = new object();
    }

    public void Initialize()
    {
    }
}
