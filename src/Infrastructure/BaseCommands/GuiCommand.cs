// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Threading;
using System.Windows;

using Media.Interfaces;

namespace Media.Infrastructure.BaseCommands;

internal abstract class GuiCommand<TWindow> : Command where TWindow : Window, new()
{
    protected virtual IViewModel? CreateDataContext(IUiFunctions uiFunctions) => null;

    protected virtual IWindowManipulator? CreateWindowManipulator() => null;

    protected virtual void ThreadCode()
    {
        try
        {
            var runner = new AppRunner<TWindow>();

            var customManipulator = CreateWindowManipulator();

            if (customManipulator != null)
            {
                runner.WindowManipulator = customManipulator;
            }

            runner.Run(CreateDataContext(new UiFunctionsImplementation()));

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
            GlobalExceptionHandler.HandleExcpetion(ex);
        }
    }

    public override int Execute(CommandContext context)
    {
        Terminal.InfoText($"Starting Ui Thread...");
        Thread uiThread = new(ThreadCode);
        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.Start();
        Terminal.InfoText($"Ui Thread started. Close window to return to command line");
        uiThread.Join();
        return ExitCodes.Success;
    }
}
