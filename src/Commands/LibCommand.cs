using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Ui;

namespace Media.Commands;

internal sealed class LibCommand : GuiCommandBase<LibWindow>
{
    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions)
    {
        return new LibWindowViewModel();
    }
}
