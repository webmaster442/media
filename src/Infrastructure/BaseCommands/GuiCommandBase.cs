using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Media.Infrastructure.BaseCommands;

internal abstract class GuiCommandBase<TWindow> : Command where TWindow : Window, new()
{
    protected virtual INotifyPropertyChanged? CreateDataContext() => null;

    protected virtual Point? GetWindowStartLocation(Size screen, Size window)
    {
        return new Point
        {
            X = (screen.Width - window.Width) / 2,
            Y = (screen.Height - window.Height) / 2
        };
    }

    private void UiThreadCode()
    {
        Application app = new();
        app.DispatcherUnhandledException += OnException;
        app.Resources.MergedDictionaries.Add(new ResourceDictionary {  Source = new("pack://application:,,,/Media;component/Ui/GlobalStyles.xaml") });
        app.ShutdownMode = ShutdownMode.OnMainWindowClose;
        app.MainWindow = new TWindow();

        var dataContext = CreateDataContext();
        if (dataContext != null)
            app.MainWindow.DataContext = dataContext;

        app.MainWindow.SnapsToDevicePixels = true;
        app.MainWindow.UseLayoutRounding = true;

        Point? position = GetWindowStartLocation(new Size(SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height),
                                                new Size(app.MainWindow.Width, app.MainWindow.Height));

        if (position != null)
        {
            app.MainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            app.MainWindow.Left = position.Value.X;
            app.MainWindow.Top = position.Value.Y;
        }

        app.MainWindow.ShowDialog();
        app.DispatcherUnhandledException -= OnException;
    }

    private void OnException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Terminal.DisplayException(e.Exception);
        MessageBox.Show(e.Exception.Message, "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
        Environment.Exit(ExitCodes.Exception);
    }

    public override int Execute(CommandContext context)
    {
        Terminal.InfoText($"Starting Ui Thread...");
        Thread uiThread = new(UiThreadCode);
        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.Start();
        Terminal.InfoText($"Ui Thread started. Close window to return to command line");
        uiThread.Join();
        return ExitCodes.Success;
    }
}
