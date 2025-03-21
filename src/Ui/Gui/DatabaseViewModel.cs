// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Collections;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Media.Database.Entity;
using Media.DbAdapters;
using Media.Infrastructure;
using Media.Interfaces;
using Media.Interop;

namespace Media.Ui.Gui;

internal partial class DatabaseViewModel : ObservableObject
{
    private readonly IUiFunctions _uiFunctions;
    private readonly GuiDatabaseAdapter _guiDatabaseAdapter;

    public delegate Task<IEnumerable> RefreshFunc();

    private RefreshFunc? _refreshTask;

    [ObservableProperty]
    public partial IEnumerable Results { get; set; }

    public DatabaseViewModel(IUiFunctions uiFunctions,
                             GuiDatabaseAdapter guiDatabaseAdapter)
    {
        _uiFunctions = uiFunctions;
        _guiDatabaseAdapter = guiDatabaseAdapter;
        Results = Enumerable.Empty<object>();
    }

    [RelayCommand]
    private async Task GetFromRange(string rangeParam)
    {
        _uiFunctions.BeginAsyncOperation();
        (DateTime start, DateTime end) range = (DateTime.MinValue, DateTime.MaxValue);
        switch (rangeParam)
        {
            case "today":
                range = DateTime.Now.Day();
                break;

            case "last3days":
                range = DateTime.Now.Last3Days();
                break;

            case "week":
                range = DateTime.Now.Week();
                break;

            case "month":
                range = DateTime.Now.Month();
                break;

            case "all":
                range = (DateTime.MinValue, DateTime.MaxValue);
                break;
        }

        _refreshTask = async() => await _guiDatabaseAdapter.GetPlayedEntries(range.start, range.end);
        if (_refreshTask != null)
        {
            Results = await _refreshTask();
        }
        _uiFunctions.EndAsyncOperation();
    }

    [RelayCommand]
    private async Task Cleanup()
        => await _guiDatabaseAdapter.ForceDbJobRunning();

    [RelayCommand]
    private async Task DropLastPlayed()
    {
        if (_uiFunctions.ConfirmMessage("Do you want to remove all last played items?", "Confirm"))
        {
            await _guiDatabaseAdapter.DropLastPlayed();
        }
    }

    private bool CanPlay(object item)
        => item is PlayedEntry playedEntry && File.Exists(playedEntry.Path);

    [RelayCommand(CanExecute = nameof(CanPlay))]
    private void Play(object item)
    {
        if (item is PlayedEntry playedEntry)
        {
            SelfInterop.Play(playedEntry.Path);
        }
    }

    [RelayCommand(CanExecute = nameof(CanPlay))]
    public void SendToPlaylist(object item)
    {
        if (item is PlayedEntry playedEntry)
        {
            WeakReferenceMessenger.Default.Send(new PlaylistViewModel.AddToPlaylistMessage
            {
                FullPath = playedEntry.Path
            });
        }
    }
}