using System.Windows;

using Media.Infrastructure.BaseCommands;
using Media.Ui;

namespace Media.Commands;

internal class DropConvert : GuiCommandBase<DropConvertWindow>
{
    protected override Point? GetWindowStartLocation(Size screen, Size window)
    {
        return new Point
        {
            X = (screen.Width - window.Width) - 10,
            Y = (screen.Height - window.Height) - 10
        };
    }
}
