// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Interop.CdRip;

namespace Media.Commands;

[Example("Eject cd drive d:", @"media cdeject D:\")]
internal sealed class CdEject : AsyncCommand<BaseCdSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, BaseCdSettings settings)
    {
        using var drive = CdDrive.Create(settings.DriveLetter);
        var discInDrive = await drive.IsCdInDriveAsync();
        if (discInDrive)
        {
            await drive.UnLockAsync();
        }
        await drive.EjectAsync();
        return ExitCodes.Success;
    }
}
