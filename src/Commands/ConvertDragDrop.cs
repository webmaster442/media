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

internal class ConvertDragDrop : GuiCommandBase<DropConvertWindow>
{
    protected override Point? GetWindowStartLocation(Size screen, Size window)
    {
        return new Point
        {
            X = (screen.Width - window.Width) - 10,
            Y = (screen.Height - window.Height) - 10
        };
    }

    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions)
        => new DropConvertViewModel(uiFunctions, new ConfigAccessor());
}
