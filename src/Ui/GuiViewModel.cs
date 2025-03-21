// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.DbAdapters;
using Media.Infrastructure;
using Media.Interfaces;
using Media.Interop;
using Media.Ui.Gui;

using Microsoft.Extensions.Logging;

namespace Media.Ui;

internal partial class GuiViewModel : ObservableObject, IViewModel
{
    private readonly IUiFunctions _uiFunctions;
    private readonly ConfigAdapter _configAdapter;

    public FilesViewModel FilesViewModel { get; }

    public RadioStationsViewModel RadioStationsViewModel { get; }

    public SystemMenuViewModel System { get; }

    public PlaylistViewModel PlaylistViewModel { get; }

    public AudioViewModel AudioViewModel { get; }

    public DatabaseViewModel DatabaseViewModel { get; }

    public GuiViewModel(IUiFunctions uiFunctions,
                        RadioStationsClient radioStationsClient,
                        GuiDatabaseAdapter guiDatabaseAdapter,
                        ConfigAdapter configAdapter,
                        ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger<GuiViewModel>();
        FilesViewModel = new FilesViewModel(uiFunctions, configAdapter, guiDatabaseAdapter, loggerFactory);
        System = new SystemMenuViewModel();
        RadioStationsViewModel = new RadioStationsViewModel(radioStationsClient, uiFunctions, loggerFactory);
        PlaylistViewModel = new PlaylistViewModel(uiFunctions);
        AudioViewModel = new AudioViewModel(loggerFactory);
        DatabaseViewModel = new DatabaseViewModel(uiFunctions, guiDatabaseAdapter);
        _uiFunctions = uiFunctions;
        _configAdapter = configAdapter;
    }

    [ObservableProperty]
    public partial bool AllwaysOnTop { get; set; }

    public ILogger Logger { get; }

    partial void OnAllwaysOnTopChanged(bool value)
        => _configAdapter.AlwaysOnTop = value;

    public void Initialize()
    {
        FilesViewModel.Initialize();
        RadioStationsViewModel.Initialize();
        AudioViewModel.Initialize();
        AllwaysOnTop = _configAdapter.AlwaysOnTop;
    }

    [RelayCommand]
    private void MediaCommand(string cli)
    {
        var args = cli.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        _uiFunctions.BringConsoleWindowToFront();
        SelfInterop.RunMediaCommand(args);
    }

    [RelayCommand]
    private void ImgView()
        => SelfInterop.RunMediaCommand("imgview", FilesViewModel.CurrentPath);

    [RelayCommand]
    private void RandomPlay(string count = "1")
    {
        _uiFunctions.BringConsoleWindowToFront();
        SelfInterop.RunMediaCommand("play", "random", FilesViewModel.CurrentPath, count);
    }

    [RelayCommand]
    private void Serve()
    {
        _uiFunctions.BringConsoleWindowToFront();
        SelfInterop.RunMediaCommand("serve", FilesViewModel.CurrentPath);
    }
}
