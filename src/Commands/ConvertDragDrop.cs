// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows;

using Media.DbAdapters;
using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Ui;

namespace Media.Commands;

[Example("Open a window to convert a file by drag and drop", "media convert drop")]
internal class ConvertDragDrop : GuiCommand<DropConvertWindow>
{
    private readonly ConfigAdapter _configAccessor;

    internal class DropWindowManipulator : IWindowManipulator
    {
        public Size GetWindowSize(Size xamlDefinedWindowSize, Size workArea)
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

    public ConvertDragDrop(ConfigAdapter configAccessor)
    {
        _configAccessor = configAccessor;
    }

    protected override IWindowManipulator? CreateWindowManipulator()
        => new DropWindowManipulator();

    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions)
        => new DropConvertViewModel(uiFunctions, _configAccessor);
}
