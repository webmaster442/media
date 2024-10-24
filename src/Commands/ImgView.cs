using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Infrastructure.Validation;
using Media.Interfaces;
using Media.Ui;

namespace Media.Commands;

internal class ImgView : GuiCommand<ImageViewerWindow, ImgView.Settings>
{
    internal class Settings : ValidatedCommandSettings
    {
        [DirectoryExists]
        [CommandArgument(0, "[folder]")]
        [Description("The folder to play from")]
        public string Folder { get; set; } = Environment.CurrentDirectory;
    }

    protected override IViewModel? CreateDataContext(Settings settings, IUiFunctions uiFunctions)
        => new ImageViewerViewModel(settings.Folder);
}
