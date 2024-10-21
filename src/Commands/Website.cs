// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;

namespace Media.Commands;

internal sealed class Website : Command
{
    public override int Execute(CommandContext context)
    {
        Process.Start(new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = "https://github.com/webmaster442/media"
        });
        return ExitCodes.Success;
    }
}
