// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

namespace Media.Commands;

[Example("Add files to a playlist", "media playlist add -p test.m3u audio.mp3")]
[Example("Add multiple files to a playlist", "media playlist add -p test.m3u track*.mp3")]
internal sealed class PlaylistAdd : BaseFileWorkCommand<PlaylistAdd.Settings>
{
    public class Settings : BasePlalistSettings
    {
        [CommandArgument(0, "<file or file pattern>")]
        [Description("File or file pattern (eg. *.mp3) to add to the playlist")]
        [Required]
        public string FileToAdd { get; set; } = string.Empty;
    }

    private static int AddRange(List<string> items, IEnumerable<string> newItems)
    {
        int startCount = items.Count;
        items.AddRange(newItems);
        return items.Count - startCount;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var list = new List<string>();
        await list.LoadFromFile(settings.PlaylistName);
        var files = GetFiles(settings.FileToAdd);


        int count = AddRange(list, files);

        await list.SaveToFile(settings.PlaylistName, false);

        Terminal.GreenText($"Added {count} files to playlist {settings.PlaylistName}");

        return ExitCodes.Success;
    }
}
