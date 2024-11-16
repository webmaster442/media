// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

namespace Media.Commands;

[Example("Clear a playlist", "media playlist clear -p test.m3u")]
internal sealed class PlaylistClear : BaseFileWorkCommand<BasePlalistSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, BasePlalistSettings settings)
    {
        var list = new List<string>();
        await list.LoadFromFile(settings.PlaylistName);

        int count = list.Count;
        list.Clear();

        await list.SaveToFile(settings.PlaylistName, false);

        Terminal.GreenText($"Removed {count} files from playlist {settings.PlaylistName}");

        return ExitCodes.Success;
    }
}
