// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

[Example("Remove a file from a playlist", "media playlist remove -p test.m3u file.mp3")]
internal sealed class PlaylistRemove : BasePlaylistCommand<PlaylistRemove.Settings>
{
    public class Settings : BasePlalistSettings
    {
        [CommandArgument(0, "<file or file pattern>")]
        [Description("File or file pattern (eg. *.mp3) to remove from the playlist")]
        [Required]
        public string FileToRemove { get; set; } = string.Empty;
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        var list = await LoadFromFile(settings.PlaylistName);
        var files = GetFiles(settings.FileToRemove);

        int count = 0;
        foreach (var file in files)
        {
            if (list.Remove(file))
            {
                count++;
            }
        }

        await SaveToFile(list, settings.PlaylistName, false);

        Terminal.GreenText($"Removed {count} files from playlist {settings.PlaylistName}");
    }
}