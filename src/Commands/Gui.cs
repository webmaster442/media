// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows;

using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Ui;

namespace Media.Commands;
internal sealed class Gui : GuiCommand<GuiWindow>
{
    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions)
    {
        return new GuiViewModel(uiFunctions);
    }

    protected override IWindowManipulator? CreateWindowManipulator()
        => new WindowManipulator();

    internal class WindowManipulator : IWindowManipulator
    {
        public Size GetWindowSize(Size xamlDefinedWindowSize, Size workArea)
        {
            if (workArea.Width > 1920 && workArea.Height > 720)
            {
                return new Size(1920, 1080);
            }
            return xamlDefinedWindowSize;
        }

        public Point GetWindowStartupLocation(Size workArea, Size windowSize)
        {
            return new Point
            {
                X = (workArea.Width - windowSize.Width) / 2,
                Y = (workArea.Height - windowSize.Height) / 2
            };
        }
    }
}
