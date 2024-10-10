using System.Threading;
using System.Windows;

namespace Media.Commands;

internal class DropConvertCommand : Command
{
    public void Run(Window window, INotifyPropertyChanged? dataContext = null)
    {
        Application app = new();
        app.ShutdownMode = ShutdownMode.OnMainWindowClose;
        app.MainWindow = window;
        if (dataContext != null)
            app.MainWindow.DataContext = dataContext;
        app.MainWindow.ShowDialog();
    }

    public override int Execute(CommandContext context)
    {
        Thread uiThread = new Thread(p => Run(new Ui.DropConvertWindow()));
        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.Start();
        uiThread.Join();
        return ExitCodes.Success;
    }
}
