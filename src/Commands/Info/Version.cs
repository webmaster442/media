// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.CommandAttributes;

namespace Media.Commands.Info;

[Example("Show version information", "media version")]
internal sealed class Version : Command
{
    public override int Execute(CommandContext context)
    {
        var currentVersion = typeof(Program).Assembly.GetName().Version;
        if (currentVersion == null)
        {
            Terminal.RedText("Version was not set during build");
            return ExitCodes.Error;
        }
        Terminal.GreenText($"Version: {currentVersion}");
        return ExitCodes.Success;
    }
}
