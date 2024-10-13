using CommunityToolkit.Mvvm.ComponentModel;

using Media.Interfaces;

namespace Media.Ui;

internal sealed partial class LibWindowViewModel : ObservableObject, IViewModel
{
    private readonly IUiFunctions _uiFunctions;

    [ObservableProperty]
    private object _contentItem;

    public LibWindowViewModel(IUiFunctions uiFunctions)
    {
        _contentItem = new object();
        _uiFunctions = uiFunctions;
    }

    public void Initialize()
    {
    }
}
