using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Media.Ui.Gui;

internal partial class LogViewModel : ObservableObject
{
    private ObservableCollection<string> Messages { get; }

    public LogViewModel()
    {
        Messages = new ObservableCollection<string>();
        WeakReferenceMessenger.Default.Register<Exception>(this, OnExceptionReceived);
    }

    private void OnExceptionReceived(object recipient, Exception message)
    {
        Messages.Add(message.Message);
    }

    [RelayCommand]
    public void Clear()
    {
        Messages.Clear();
    }
}
