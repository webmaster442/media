using System.Windows.Automation;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Interop;

namespace Media.Ui.Gui;

internal sealed partial class SystemMenuViewModel : ObservableObject
{
    [RelayCommand]
    private void DisplayInternal() =>
        Windows.DisplaySwitch(Windows.DisplaySwitchMode.Internal);

    [RelayCommand]
    private void DisplayExernal() =>
        Windows.DisplaySwitch(Windows.DisplaySwitchMode.Exernal);

    [RelayCommand]
    private void DisplayExtend() =>
        Windows.DisplaySwitch(Windows.DisplaySwitchMode.Extended);

    [RelayCommand]
    private void DisplayClone() =>
        Windows.DisplaySwitch(Windows.DisplaySwitchMode.Clone);
}
