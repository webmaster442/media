// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows;

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Ui;

namespace Media.Commands;

[Example("Open a window to convert a file by drag and drop", "media convert drop")]
internal class ConvertDragDrop : GuiCommand<DropConvertWindow>
{
    internal class DropWindowManipulator : IWindowManipulator
    {
        public Size GetWindowSize(Size xamlDefinedWindowSize)
            => xamlDefinedWindowSize;

        public Point GetWindowStartupLocation(Size workArea, Size windowSize)
        {
            return new Point
            {
                X = (workArea.Width - windowSize.Width) - 10,
                Y = (workArea.Height - windowSize.Height) - 10
            };
        }
    }

    protected override IWindowManipulator? CreateWindowManipulator()
        => new DropWindowManipulator();

    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions)
        => new DropConvertViewModel(uiFunctions, new ConfigAccessor());
}
