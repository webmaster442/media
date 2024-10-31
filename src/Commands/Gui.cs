using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Ui;

namespace Media.Commands;
internal sealed class Gui : GuiCommand<GuiWindow>
{
    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions)
    {
        return new GuiViewModel(uiFunctions, new Infrastructure.ConfigAccessor());
    }
}
