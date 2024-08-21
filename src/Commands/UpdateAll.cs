// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Commands;

internal sealed class UpdateAll : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        await new UpdateMedia().ExecuteAsync(context);
        await new UpdateFFMpeg().ExecuteAsync(context);
        await new UpdateMpv().ExecuteAsync(context);
        await new UpdateYtdlp().ExecuteAsync(context);

        return ExitCodes.Success;
    }
}