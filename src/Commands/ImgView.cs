// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.BaseCommands;
using Media.Infrastructure;
using Media.Infrastructure.CommandAttributes;
using Media.Infrastructure.Validation;
using Media.Interfaces;
using Media.Ui;

using Microsoft.Extensions.Logging;

namespace Media.Commands;

[InstallerData(Arguments = "imgview", IconIndex =3, Name = "Media Image Viewer")]
internal class ImgView : BaseGuiCommand<ImageViewerWindow, ImgView.Settings>
{
    internal class Settings : ValidatedCommandSettings
    {
        [DirectoryExists]
        [CommandArgument(0, "[folder]")]
        [Description("The folder to play from")]
        public string Folder { get; set; } = Environment.CurrentDirectory;
    }

    protected override IViewModel? CreateDataContext(Settings settings, IUiFunctions uiFunctions, ILoggerFactory loggerFactory)
        => new ImageViewerViewModel(settings.Folder, uiFunctions, loggerFactory);
}
