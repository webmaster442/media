// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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

        var mainWin = App.Current.MainWindow;
        if (mainWin.TaskbarItemInfo == null)
            mainWin.TaskbarItemInfo = new TaskbarItemInfo();

        mainWin.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
        mainWin.TaskbarItemInfo.ProgressValue = value;
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

        var mainWin = App.Current.MainWindow;
        if (mainWin.TaskbarItemInfo == null)
            mainWin.TaskbarItemInfo = new TaskbarItemInfo();

        mainWin.TaskbarItemInfo.ProgressState = Map(state);

    }

    public void WarningMessage(string message, string title)
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);

    public void BlockUi()
    {
        var blocker = FindLogicalChildren<AsyncBlocker>(App.Current.MainWindow).FirstOrDefault();
        if (blocker != null)
        {
            blocker.Show();
        }
        else
        {
            var grid = FindLogicalChildren<Grid>(App.Current.MainWindow).FirstOrDefault();
            if (grid != null)
            {
                var ctrl = new AsyncBlocker();
                grid.Children.Add(ctrl);
                ctrl.Show();
            }
        }
        SetProgressState(ProgressState.Indeterminate);
    }

    public void UnblockUi()
    {
        var blocker = FindLogicalChildren<AsyncBlocker>(App.Current.MainWindow).FirstOrDefault();
        blocker?.Hide();
        SetProgressState(ProgressState.None);
    }

    private static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj != null)
        {
            foreach (object rawChild in LogicalTreeHelper.GetChildren(depObj))
            {
                if (rawChild is DependencyObject child)
                {
                    if (child is T casted)
                    {
                        yield return casted;
                    }

                    foreach (T childOfChild in FindLogicalChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }

    public string? OpenFileDialog(string filterString)
    {
        var ofd = new Microsoft.Win32.OpenFileDialog
        {
            Filter = filterString
        };
        if (ofd.ShowDialog() == true)
        {
            return ofd.FileName;
        }
        return null;
    }

    public string? SaveFileDialog(string filterString)
    {
        var sfd = new Microsoft.Win32.SaveFileDialog
        {
            Filter = filterString
        };
        if (sfd.ShowDialog() == true)
        {
            return sfd.FileName;
        }
        return null;
    }
}