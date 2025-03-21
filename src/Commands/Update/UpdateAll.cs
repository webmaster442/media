// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.DbAdapters;
using Media.Infrastructure;
using Media.Infrastructure.CommandAttributes;

namespace Media.Commands.Update;

[Example("Update all tools", "media update all")]
internal sealed class UpdateAll : AsyncCommand
{
    private readonly ConfigAdapter _configAccessor;

    public UpdateAll(ConfigAdapter configAccessor)
    {
        _configAccessor = configAccessor;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        await new UpdateMedia().ExecuteAsync(context);
        await new UpdateFFMpeg(_configAccessor).ExecuteAsync(context);
        await new UpdateMpv(_configAccessor).ExecuteAsync(context);
        await new UpdateYtdlp(_configAccessor).ExecuteAsync(context);

        return ExitCodes.Success;
    }
}