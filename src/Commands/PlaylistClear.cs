// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

[Example("Clear a playlist", "media playlist clear -p test.m3u")]
internal sealed class PlaylistClear : BasePlaylistCommand<BasePlalistSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, BasePlalistSettings settings)
    {
        var list = await LoadFromFile(settings.PlaylistName);

        int count = list.Count;
        list.Clear();

        await SaveToFile(list, settings.PlaylistName, false);

        Terminal.GreenText($"Removed {count} files from playlist {settings.PlaylistName}");

        return ExitCodes.Success;
    }
}
