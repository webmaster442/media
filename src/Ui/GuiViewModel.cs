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

    public GuiViewModel(IUiFunctions uiFunctions)
    {
        FilesViewModel = new FilesViewModel(uiFunctions);
        System = new SystemMenuViewModel();
        RadioStationsViewModel = new RadioStationsViewModel(new RadioStationsClient(), uiFunctions);
        PlaylistViewModel = new PlaylistViewModel(uiFunctions);
    }

    public void Initialize()
    {
        FilesViewModel.RefreshDriveList();
        RadioStationsViewModel.Initialize();
    }

    [RelayCommand]
    public void MediaCommand(string cli) 
        => SelfInterop.RunMediaCommand(cli);

    [RelayCommand]
    public void ImgView(string folder)
        => SelfInterop.RunMediaCommand($"imgview \"{folder}\"");

    [RelayCommand]
    public void Serve(string folder)
        => SelfInterop.RunMediaCommand($"serve \"{folder}\"");
}
