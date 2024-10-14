using Media.Database;
using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Ui;

namespace Media.Commands;

internal sealed class MediaLib : GuiCommandBase<LibWindow>
{
    private readonly MediaDbSerives _mediaDbSerives;

    public MediaLib(MediaDbSerives mediaDbSerives)
    {
        _mediaDbSerives = mediaDbSerives;
    }

    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions)
    {
        return new LibWindowViewModel(uiFunctions, _mediaDbSerives);
    }
}
