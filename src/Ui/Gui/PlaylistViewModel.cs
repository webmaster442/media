// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Media.Database.Entity;
using Media.Infrastructure;
using Media.Interfaces;
using Media.Interop;

namespace Media.Ui.Gui;

internal sealed partial class PlaylistViewModel : ObservableObject
{
    public sealed class AddToPlaylistMessage
    {
        public required string FullPath { get; init; }
    }


    public BindingList<string> PlaylistItems { get; }

    private readonly IUiFunctions _uiFunctions;


    public PlaylistViewModel(IUiFunctions uiFunctions)
    {
        PlaylistItems = new BindingList<string>();
        _uiFunctions = uiFunctions;
        WeakReferenceMessenger.Default.Register<AddToPlaylistMessage>(this, OnAddToPlaylist);
    }

    private void OnAddToPlaylist(object recipient, AddToPlaylistMessage message)
        => PlaylistItems.Add(message.FullPath);

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
    private async Task PlayCurrentList()
    {
        static string CreateTempName() 
            => $"playlist_{DateTime.Now:yyyyMMddHHmmss}.m3u";

        var tempFile = Path.Combine(Path.GetTempPath(), CreateTempName());
        await PlaylistItems.SaveToFile(tempFile, relativePaths: false);

        SelfInterop.Play(tempFile);

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
            await PlaylistItems.SaveToFile(selectedFile, relativePaths: false);
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
        var ordered = PlaylistItems.Order().ToList();
        PlaylistItems.Clear();
        PlaylistItems.AddRange(ordered);
    }

    [RelayCommand]
    private void OrderZa()
    {
        var ordered = PlaylistItems.OrderDescending().ToList();
        PlaylistItems.Clear();
        PlaylistItems.AddRange(ordered);
    }

    private bool CanPlay(string item)
        => File.Exists(item);

    [RelayCommand(CanExecute = nameof(CanPlay))]
    private void Play(string item)
        => SelfInterop.Play(item);

    [RelayCommand]
    private void Remove(string item)
        => PlaylistItems.Remove(item);
}
