using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Media.Infrastructure;
using Media.Interfaces;
using Media.Interop;

namespace Media.Ui.Gui;

internal partial class PlaylistViewModel : ObservableObject
{
    public BindingList<string> PlaylistItems { get; }

    private readonly IUiFunctions _uiFunctions;
    

    public PlaylistViewModel(IUiFunctions uiFunctions)
    {
        PlaylistItems = new BindingList<string>();
        _uiFunctions = uiFunctions;
        WeakReferenceMessenger.Default.Register<FolderItem>(this, OnFolderItemRecieved);
    }

    private void OnFolderItemRecieved(object recipient, FolderItem message)
    {
        PlaylistItems.Add(message.FullPath);
    }

    [RelayCommand]
    private async Task Load()
    {
        string? selectedFile = _uiFunctions.OpenFileDialog("Playlist files (*.m3u, *.pls)|*.m3u;*.pls");
        if (selectedFile is not null)
        {
            PlaylistItems.RaiseListChangedEvents = false;
            await PlaylistItems.LoadFromFile(selectedFile);
            PlaylistItems.RaiseListChangedEvents = true;
            PlaylistItems.ResetBindings();
        }
    }

    [RelayCommand]
    private void Clear()
    {
        PlaylistItems.Clear();
    }

    [RelayCommand]
    private async Task Save()
    {
        string? selectedFile = _uiFunctions.SaveFileDialog("pls playlist|*.pls|m3u playlist|*.m3u");
        if (selectedFile is not null)
        {
            await PlaylistItems.SaveToFile(selectedFile, false);
        }
    }

    [RelayCommand]
    private void Shuffle()
    {
        PlaylistItems.RaiseListChangedEvents = false;
        PlaylistItems.Shuffle();
        PlaylistItems.RaiseListChangedEvents = true;
        PlaylistItems.ResetBindings();
    }

    [RelayCommand]
    private void OrderAz()
    {
        PlaylistItems.RaiseListChangedEvents = false;

        var ordered = PlaylistItems.Order().ToList();
        PlaylistItems.Clear();
        PlaylistItems.AddRange(ordered);

        PlaylistItems.RaiseListChangedEvents = true;
        PlaylistItems.ResetBindings();
    }

    [RelayCommand]
    private void OrderZa()
    {
        PlaylistItems.RaiseListChangedEvents = false;

        var ordered = PlaylistItems.OrderDescending().ToList();
        PlaylistItems.Clear();
        PlaylistItems.AddRange(ordered);

        PlaylistItems.RaiseListChangedEvents = true;
        PlaylistItems.ResetBindings();
    }
}
