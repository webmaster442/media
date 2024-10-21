// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

[Example("Create a new playlist", "media playlist new -p test.m3u")]
internal sealed class PlaylistNew : BasePlaylistCommand<PlaylistNew.Settings>
{
    internal class Settings : ValidatedCommandSettings
    {
        [Description("Playlist file")]
        [CommandOption("-p|--playlist")]
        [Required]
        public string PlaylistName { get; set; }

        public Settings()
        {
            var files = Directory.GetFiles(Environment.CurrentDirectory, "*.m3u");
            PlaylistName = files.Length == 0 ? MakeProjectName(Environment.CurrentDirectory) : string.Empty;
        }

        private static string MakeProjectName(string currentDirectory)
        {
            var name = Path.GetFileName(currentDirectory);
            return Path.Combine(currentDirectory, $"{Path.ChangeExtension(name, ".m3u")}");
        }
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        var list = new Playlist();

        await SaveToFile(list, settings.PlaylistName, false);

        Terminal.GreenText($"Created playlist {settings.PlaylistName}");
    }
}
