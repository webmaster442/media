// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Ui;

namespace Media.Commands;
internal sealed class Gui : GuiCommand<GuiWindow>
{
    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions)
    {
        return new GuiViewModel(uiFunctions);
    }
}
