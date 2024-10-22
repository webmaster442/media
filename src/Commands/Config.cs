// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;

using Media.Infrastructure;

namespace Media.Commands;

[Example("Open the config file in the default editor", "media config")]
internal sealed class Config : AsyncCommand
{
    private readonly ConfigAccessor _configAccessor;

    public Config(ConfigAccessor configAccessor)
    {
        _configAccessor = configAccessor;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {

        await _configAccessor.ForceSave();

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _configAccessor.ConfigPath,
                UseShellExecute = true,
            },
        };
        process.Start();

        return ExitCodes.Success;

    }
}
