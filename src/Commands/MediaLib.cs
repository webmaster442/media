using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Ui;

namespace Media.Commands;

internal sealed class MediaLib : GuiCommandBase<LibWindow>
{
    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions)
    {
        return new LibWindowViewModel(uiFunctions);
    }
}
