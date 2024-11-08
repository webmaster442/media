// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;

using Media.Interfaces;
using Media.Ui.Controls;

namespace Media.Infrastructure;

internal class UiFunctionsImplementation : IUiFunctions
{
    public void ErrorMessage(string message, string title)
    {
        Terminal.RedText(message);
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void Exit(int exitCode)
        => Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown(exitCode));

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

    public void Report(double value)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var mainWin = App.Current.MainWindow;
            if (mainWin.TaskbarItemInfo == null)
                mainWin.TaskbarItemInfo = new TaskbarItemInfo();

            mainWin.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            mainWin.TaskbarItemInfo.ProgressValue = value;
        });
    }

    public void SetProgressState(ProgressState state)
    {
        static TaskbarItemProgressState Map(ProgressState state)
        {
            return state switch
            {
                ProgressState.None => TaskbarItemProgressState.None,
                ProgressState.Indeterminate => TaskbarItemProgressState.Indeterminate,
                ProgressState.Normal => TaskbarItemProgressState.Normal,
                ProgressState.Error => TaskbarItemProgressState.Error,
                ProgressState.Paused => TaskbarItemProgressState.Paused,
                _ => throw new UnreachableException("Can't map Progress state"),
            };
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
            var mainWin = App.Current.MainWindow;
            if (mainWin.TaskbarItemInfo == null)
                mainWin.TaskbarItemInfo = new TaskbarItemInfo();

            mainWin.TaskbarItemInfo.ProgressState = Map(state);
        });
    }

    public void WarningMessage(string message, string title)
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);

    public void BlockUi()
    {
        var blocker = FindElement<AsyncBlocker>(App.Current.MainWindow);
        if (blocker != null)
        {
            blocker.Show();
        }
        else
        {
            var grid = FindElement<Grid>(App.Current.MainWindow);
            if (grid != null)
            {
                var ctrl = new AsyncBlocker();
                grid.Children.Add(ctrl);
                ctrl.Show();
            }
        }
    }

    public void UnblockUi()
    {
        var blocker = FindElement<AsyncBlocker>(App.Current.MainWindow);
        blocker?.Hide();
    }

    private static T? FindElement<T>(DependencyObject obj)
        where T: UIElement
    {
        if (obj is T casted)
            return casted;

        int count = VisualTreeHelper.GetChildrenCount(obj);

        DependencyObject? foundChild = null;

        for (int i=0; i<count; i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            foundChild = FindElement<T>(child);
            if (foundChild != null) break;
        }

        return foundChild as T;
    }
}