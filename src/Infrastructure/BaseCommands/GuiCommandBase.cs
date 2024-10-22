// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Media.Interfaces;

namespace Media.Infrastructure.BaseCommands;

internal abstract class GuiCommandBase<TWindow> : Command where TWindow : Window, new()
{
    internal class UiFunctionsImplementation : IUiFunctions
    {
        public void ErrorMessage(string message, string title)
        {
            Terminal.RedText(message);
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Exit(int exitCode)
        {
            Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown(exitCode));
        }

        public void InfoMessage(string message, string title)
            => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

        public bool QuestionMessage(string message, string title)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public string? SelectFolderDialog(string? startFolder)
        {
            var selector = new Microsoft.Win32.OpenFolderDialog();
            if (!string.IsNullOrWhiteSpace(startFolder))
                selector.DefaultDirectory = startFolder;
            if (selector.ShowDialog() == true)
            {
                return selector.FolderName;
            }
            return null;
        }

        public void WarningMessage(string message, string title)
            => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }


    protected virtual IViewModel? CreateDataContext(IUiFunctions uiFunctions) => null;

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
        App app = new();
        app.InitializeComponent();
        app.DispatcherUnhandledException += OnException;
        app.ShutdownMode = ShutdownMode.OnMainWindowClose;
        app.MainWindow = new TWindow();

        var dataContext = CreateDataContext(new UiFunctionsImplementation());
        if (dataContext != null)
        {
            app.MainWindow.DataContext = dataContext;
            dataContext.Initialize();
        }

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
        GlobalExceptionHandler.HandleExcpetion(e.Exception);
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
