
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Media.Interfaces;
using Media.Ui.Controls;

namespace Media.Ui.Gui;

internal partial class PlaylistViewModel : ObservableObject
{
    public ObservableRangeCollection<string> PlaylistItems;
    private readonly IUiFunctions _uiFunctions;

    public PlaylistViewModel(IUiFunctions uiFunctions)
    {
        PlaylistItems = new ObservableRangeCollection<string>();
        _uiFunctions = uiFunctions;
        WeakReferenceMessenger.Default.Register<FolderItem>(this, OnFolderItemRecieved);
    }

    private void OnFolderItemRecieved(object recipient, FolderItem message)
    {
        PlaylistItems.Add(message.FullPath);
    }

    [RelayCommand]
    public void Load()
    {
        string? selectedFile = _uiFunctions.OpenFileDialog("Playlist files (*.m3u, *.pls)|*.m3u;*.pls");
        if (selectedFile is not null)
        {
        }
    }

    [RelayCommand]
    public void Clear()
    {
        PlaylistItems.Clear();
    }

    [RelayCommand]
    public void SavePls()
    {
        string? selectedFile = _uiFunctions.SaveFileDialog("Playlist files (*.pls)|*.pls");
        if (selectedFile is not null)
        {
        }
    }

    [RelayCommand]
    public void SaveM3u()
    {
        string? selectedFile = _uiFunctions.SaveFileDialog("Playlist files (*.m3u)|*.m3u");
        if (selectedFile is not null)
        {
        }
    }
}
