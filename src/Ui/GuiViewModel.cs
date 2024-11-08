using CommunityToolkit.Mvvm.ComponentModel;

using Media.Infrastructure;
using Media.Interfaces;
using Media.Ui.Gui;

namespace Media.Ui;

internal class GuiViewModel : ObservableObject, IViewModel
{
    private readonly IUiFunctions _uiFunctions;

    public FilesViewModel FilesViewModel { get; }

    public RadioStationsViewModel RadioStationsViewModel { get; }

    public SystemMenuViewModel System { get; }

    public GuiViewModel(IUiFunctions uiFunctions)
    {
        _uiFunctions = uiFunctions;
        FilesViewModel = new FilesViewModel(uiFunctions);
        System = new SystemMenuViewModel();
        RadioStationsViewModel = new RadioStationsViewModel(new RadioStationsClient(), uiFunctions);
    }

    public void Initialize()
    {
        FilesViewModel.RefreshDriveList();
        RadioStationsViewModel.Initialize();
    }
}
