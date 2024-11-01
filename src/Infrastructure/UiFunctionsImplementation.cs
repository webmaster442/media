// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Windows;
using System.Windows.Shell;

using Media.Interfaces;

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
}