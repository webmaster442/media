// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Infrastructure;
using Media.Interfaces;
using Media.Interop;
using Media.Ui.Gui;

namespace Media.Ui;

internal partial class GuiViewModel : ObservableObject, IViewModel
{
    public FilesViewModel FilesViewModel { get; }

    public RadioStationsViewModel RadioStationsViewModel { get; }

    public SystemMenuViewModel System { get; }

    public PlaylistViewModel PlaylistViewModel { get; }

    public AudioViewModel AudioViewModel { get; }

    public GuiViewModel(IUiFunctions uiFunctions)
    {
        FilesViewModel = new FilesViewModel(uiFunctions);
        System = new SystemMenuViewModel();
        RadioStationsViewModel = new RadioStationsViewModel(new RadioStationsClient(), uiFunctions);
        PlaylistViewModel = new PlaylistViewModel(uiFunctions);
        AudioViewModel = new AudioViewModel();
    }

    public void Initialize()
    {
        FilesViewModel.RefreshDriveList();
        RadioStationsViewModel.Initialize();
        AudioViewModel.Initialize();
    }

    [RelayCommand]
    private void MediaCommand(string cli)
    {
        var args = cli.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        SelfInterop.RunMediaCommand(args);
    }

    [RelayCommand]
    private void ImgView(string folder)
        => SelfInterop.RunMediaCommand("imgview", folder);

    [RelayCommand]
    private void RandomPlay(string folder)
        => SelfInterop.RunMediaCommand("play", "random", folder);

    [RelayCommand]
    private void Serve(string folder)
        => SelfInterop.RunMediaCommand("serve", folder);
}
