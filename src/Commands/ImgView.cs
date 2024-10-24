using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Infrastructure.Validation;
using Media.Interfaces;
using Media.Ui;

namespace Media.Commands;

internal class ImgView : GuiCommandBase<ImageViewerWindow>
{
    internal class Settings : ValidatedCommandSettings
    {
        [DirectoryExists]
        [CommandArgument(0, "[folder]")]
        [Description("The folder to play from")]
        public string Folder { get; set; } = Environment.CurrentDirectory;
    }
}
