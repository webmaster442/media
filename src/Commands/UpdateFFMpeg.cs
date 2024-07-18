using FFCmd.Infrastructure;

using Spectre.Console;
using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal class UpdateFFMpeg : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        try
        {
            var client = new GithubClient();
            var releases = await client.GetReleases("BtbN", "FFmpeg-Builds");

            var latest = releases
                .Where(r => r.Prerelease == false
                    && r.Draft == false
                    && r.Assets.Length > 0)
                .OrderByDescending(r => r.PublishedAt)
                .FirstOrDefault();

            return 0;
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            return -1;
        }
    }
}
