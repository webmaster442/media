using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Media.Database.Jobs;

internal sealed class RemoveFolderBookmarkNonExisting : IDatabaseJob
{
    public TimeSpan TriggerInterval => TimeSpan.FromDays(7);

    public async Task RunJob(DatabaseContext databaseContext, ILogger logger)
    {
        List<string> deletedPaths = new();
        var dbPaths = databaseContext.FolderBookmarks.Select(x => x.Path).AsAsyncEnumerable();
        await foreach (var path in dbPaths)
        {
            if (!File.Exists(path))
            {
                deletedPaths.Add(path);
            }
        }
        foreach (var path in deletedPaths)
        {
            var entry = await databaseContext.FolderBookmarks.FirstOrDefaultAsync(x => x.Path == path);
            if (entry != null)
            {
                databaseContext.FolderBookmarks.Remove(entry);
            }
        }
        int count = await databaseContext.SaveChangesAsync();
        logger.LogInformation("Removed {count} folder bookmark entries that no longer exist", count);
    }
}
