﻿using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

internal sealed class PlaylistAdd : BasePlaylistCommand<PlaylistAdd.Settings>
{
    public class Settings : BasePlalistSettings
    {
        [CommandArgument(0, "<file or file pattern>")]
        [Description("File or file pattern (eg. *.mp3) to add to the playlist")]
        [Required]
        public string FileToAdd { get; set; } = string.Empty;
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        var list = await LoadFromFile(settings.PlaylistName);
        var files = GetFiles(settings.FileToAdd);

        int count = list.AddRange(files);

        await SaveToFile(list, settings.PlaylistName, false);

        Terminal.GreenText($"Added {count} files to playlist {settings.PlaylistName}");
    }
}
