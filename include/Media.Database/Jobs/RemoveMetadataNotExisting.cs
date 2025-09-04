using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Media.Database.Jobs;

internal sealed class RemoveMetadataNotExisting : IDatabaseJob
{
    public TimeSpan TriggerInterval => TimeSpan.FromDays(7);

    public async Task RunJob(DatabaseContext databaseContext, ILogger logger)
    {
        List<string> deletedPaths = new();
        var dbPaths = databaseContext.Metadata.Select(x => x.Path).AsAsyncEnumerable();
        await foreach (var path in dbPaths)
        {
            if (!File.Exists(path))
            {
                deletedPaths.Add(path);
            }
        }
        foreach (var path in deletedPaths)
        {
            var entry = await databaseContext.Metadata.FirstOrDefaultAsync(x => x.Path == path);
            if (entry != null)
            {
                databaseContext.Metadata.Remove(entry);
            }
        }
        int count = await databaseContext.SaveChangesAsync();
        logger.LogInformation("Removed {count} metadata entries that no longer exist", count);
    }
}
