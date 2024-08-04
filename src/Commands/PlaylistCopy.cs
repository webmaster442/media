using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

using Spectre.Console;

namespace Media.Commands;
internal class PlaylistCopy : BasePlaylistCommand<PlaylistCopy.Settings>
{
    public class Settings : BasePlalistSettings
    {
        [CommandArgument(0, "<target directory>")]
        [Description("Target directory to copy files to in the playlist.")]
        [Required]
        public string TargetDirectory { get; set; } = string.Empty;
    }

    private enum CopyResult
    {
        Copied,
        Skipped,
        Errored
    }

    private static async Task<CopyResult> CopyFile(string sourceFile, string targetDirectory, Action<long, long> progeress)
    {
        var targetFile = Path.Combine(targetDirectory, Path.GetFileName(sourceFile));
        if (File.Exists(targetFile))
        {
            return CopyResult.Skipped;
        }

        long position = 0;
        int read = 0;
        try
        {
            byte[] buffer = new byte[16 * 1024];
            using var source = File.OpenRead(sourceFile);
            using var target = File.Create(targetFile);
            do
            {
                read = await source.ReadAsync(buffer, 0, buffer.Length);
                await target.WriteAsync(buffer, 0, read);
                position += read;
                progeress.Invoke(position, source.Length);
            }
            while (read > 0);
            return CopyResult.Copied;
        }
        catch (Exception)
        {
            return CopyResult.Errored;
        }
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        var list = await LoadFromFile(settings.PlaylistName);

        int copied = 0;
        int skipped = 0;
        int failed = 0;
        await AnsiConsole.Progress().AutoRefresh(false).StartAsync(async ctx =>
        {
            foreach (var file in list)
            {
                var task = ctx.AddTask($"Copying {Path.GetFileName(file)}");
                var result = await CopyFile(file, settings.TargetDirectory, (long position, long length) =>
                {
                    task.Value = (double)position / length * 100;
                    ctx.Refresh();
                });
                switch (result)
                {
                    case CopyResult.Copied:
                        copied++;
                        break;
                    case CopyResult.Skipped:
                        skipped++;
                        break;
                    case CopyResult.Errored:
                        failed++;
                        break;
                }
                if (failed > 0)
                {
                    Terminal.RedText($"Failed to copy {failed} files");
                }
                if (skipped > 0)
                {
                    Terminal.InfoText($"Skipped {skipped} files");
                }
                if (copied > 0)
                {
                    Terminal.GreenText($"Copied {copied} files");
                }
            }
        });
    }
}
