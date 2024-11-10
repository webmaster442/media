// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Media.Interfaces;

namespace Media.Infrastructure;
internal class AppRunner<TWindow> where TWindow : Window, new()
{
    public App App { get; }

    public TWindow MainWindow
        => (App.MainWindow as TWindow)!;

    public IWindowManipulator WindowManipulator { get; set; }

    private class DefaultWindowManipulator : IWindowManipulator
    {
        public Size GetWindowSize(Size xamlDefinedWindowSize, Size workArea)
            => xamlDefinedWindowSize;

        public Point GetWindowStartupLocation(Size workArea, Size windowSize)
        {
            return new Point
            {
                X = (workArea.Width - windowSize.Width) / 2,
                Y = (workArea.Height - windowSize.Height) / 2
            };
        }
    }

    public AppRunner()
    {
        App = new();
        App.InitializeComponent();
        App.ShutdownMode = ShutdownMode.OnMainWindowClose;
        App.MainWindow = new TWindow();
        App.MainWindow.SnapsToDevicePixels = true;
        App.MainWindow.UseLayoutRounding = true;
        App.MainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
        WindowManipulator = new DefaultWindowManipulator();

        //Set WPF async await dispatcher
        if (SynchronizationContext.Current is not DispatcherSynchronizationContext)
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
    }

    public void Run(IViewModel? dataContext)
    {
        if (dataContext != null)
        {
            App.MainWindow.DataContext = dataContext;
            dataContext.Initialize();
        }

        Size screen = new Size(SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);

        Size newSize = WindowManipulator.GetWindowSize(new Size(App.MainWindow.Width, App.MainWindow.Height), screen);
        App.MainWindow.Width = newSize.Width;
        App.MainWindow.Height = newSize.Height;

        Point position = WindowManipulator.GetWindowStartupLocation(screen, newSize);

        App.MainWindow.Left = position.X;
        App.MainWindow.Top = position.Y;

        App.MainWindow.ShowDialog();
    }
}