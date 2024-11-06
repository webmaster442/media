using CommunityToolkit.Mvvm.ComponentModel;

using Media.Interfaces;
using Media.Ui.Gui;

namespace Media.Ui;

internal class GuiViewModel : ObservableObject, IViewModel
{
    private readonly IUiFunctions _uiFunctions;

    public FilesViewModel FilesViewModel { get; }
    public LogViewModel LogViewModel { get; }

    public GuiViewModel(IUiFunctions uiFunctions)
    {
        _uiFunctions = uiFunctions;
        FilesViewModel = new FilesViewModel(uiFunctions);
        LogViewModel = new LogViewModel();
    }

    public void Initialize()
    {
        FilesViewModel.RefreshDriveList();
    }
}
