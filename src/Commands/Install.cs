// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.BaseCommands;
using Media.Interfaces;
using Media.Ui;

using Microsoft.Extensions.Logging;

namespace Media.Commands;

internal sealed class Install : BaseGuiCommand<InstallWindow>
{
    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions, ILoggerFactory loggerFactory)
    {
        return new InstallWindowViewModel(uiFunctions, loggerFactory);
    }
}
