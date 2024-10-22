// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

[Example("Add files to a playlist", "media playlist add -p test.m3u audio.mp3")]
[Example("Add multiple files to a playlist", "media playlist add -p test.m3u track*.mp3")]
internal sealed class PlaylistAdd : BasePlaylistCommand<PlaylistAdd.Settings>
{
    public class Settings : BasePlalistSettings
    {
        [CommandArgument(0, "<file or file pattern>")]
        [Description("File or file pattern (eg. *.mp3) to add to the playlist")]
        [Required]
        public string FileToAdd { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var list = await LoadFromFile(settings.PlaylistName);
        var files = GetFiles(settings.FileToAdd);

        int count = list.AddRange(files);

        await SaveToFile(list, settings.PlaylistName, false);

        Terminal.GreenText($"Added {count} files to playlist {settings.PlaylistName}");

        return ExitCodes.Success;
    }
}
