using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Interfaces;

namespace Media.Ui;

internal sealed partial class ImageViewerViewModel : ObservableObject, IViewModel
{
    public ObservableCollection<string> ImageFiles { get; }

    public ImageViewerViewModel(string folder)
    {
        ImageFiles = new ObservableCollection<string>();
    }

    public void Initialize()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void Next()
    {

    }

    [RelayCommand]
    private void Previous()
    {

    }
}
