using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Media.Ui.Gui;

internal sealed partial class SystemMenuViewModel : ObservableObject
{
    [RelayCommand]
    private void DisplayInternal() =>
        Interop.Windows.DisplaySwitch(Interop.Windows.DisplaySwitchMode.Internal);

    [RelayCommand]
    private void DisplayExernal() =>
        Interop.Windows.DisplaySwitch(Interop.Windows.DisplaySwitchMode.Exernal);

    [RelayCommand]
    private void DisplayExtend() =>
        Interop.Windows.DisplaySwitch(Interop.Windows.DisplaySwitchMode.Extended);

    [RelayCommand]
    private void DisplayClone() =>
        Interop.Windows.DisplaySwitch(Interop.Windows.DisplaySwitchMode.Clone);

    [RelayCommand]
    private void Lock()
        => Interop.Windows.Lock();

    [RelayCommand]
    private void Sleep()
        => Interop.Windows.Sleep();

    [RelayCommand]
    private void Shutdown()
        => Interop.Windows.Shutdown();

    [RelayCommand]
    private void Restart()
        => Interop.Windows.Restart();

    [RelayCommand]
    private void Logout()
        => Interop.Windows.Logoff();

    [RelayCommand]
    private void Hibernate()
        => Interop.Windows.Hibernate();
}
