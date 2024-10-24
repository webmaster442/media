// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Threading;
using System.Windows;

using Media.Interfaces;

using Spectre.Console;

namespace Media.Infrastructure.BaseCommands;

internal abstract class GuiCommand<TWindow, TSettings> : Command<TSettings>
    where TWindow : Window, new() 
    where TSettings : CommandSettings
{
    protected virtual IViewModel? CreateDataContext(TSettings settings, IUiFunctions uiFunctions) => null;

    protected virtual IWindowManipulator? CreateWindowManipulator() => null;

    private void ThreadCode(object? obj)
    {
        try
        {
            TSettings settings = (TSettings)obj!;

            var runner = new AppRunner<TWindow>();

            var customManipulator = CreateWindowManipulator();

            if (customManipulator != null)
            {
                runner.WindowManipulator = customManipulator;
            }

            runner.Run(CreateDataContext(settings, new UiFunctionsImplementation()));

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
            GlobalExceptionHandler.HandleExcpetion(ex);
        }
    }

    public override int Execute(CommandContext context, TSettings settings)
    {
        AnsiConsole.Clear();
        Terminal.InfoText($"Starting Ui Thread...");
        Thread uiThread = new(ThreadCode);
        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.Start(settings);
        Terminal.InfoText($"Ui Thread started. Close window to return to command line");
        uiThread.Join();
        return ExitCodes.Success;
    }
}