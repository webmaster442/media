using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

namespace Media.Commands;

internal sealed class PlaylistClear : BasePlaylistCommand<BasePlalistSettings>
{
    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, BasePlalistSettings settings)
    {
        var list = await LoadFromFile(settings.PlaylistName);

        int count = list.Count;
        list.Clear();

        await SaveToFile(list, settings.PlaylistName, false);

        Terminal.GreenText($"Removed {count} files from playlist {settings.PlaylistName}");
    }
}
