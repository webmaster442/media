using Media.Database.Entity;

using Microsoft.EntityFrameworkCore;

namespace Media.DbAdapters;

internal class GuiDatabaseAdapter : DatabaseAdapterBase
{
    public async Task<List<PlayedEntry>> GetPlayedEntries(DateTime start, DateTime end)
    {
        using var dbContext = GetContext();
        return await dbContext.PlayedEntries
            .Where(p => p.LastPlayed <= end)
            .Where(p => p.LastPlayed >= start)
            .ToListAsync();
    }

    public async Task<List<string>> GetPlayedThatNotExists()
    {
        using var context = GetContext();
        var dbPaths = context.PlayedEntries.Select(x => x.Path).AsAsyncEnumerable();

        List<string> result = new();
        await foreach (var path in dbPaths)
        {
            if (!System.IO.File.Exists(path))
            {
                result.Add(path);
            }
        }
        return result;
    }

    public async Task RemovePlayedEntries(IEnumerable<string> files)
    {
        using var context = GetContext();
        foreach (var file in files)
        {
            var entry = await context.PlayedEntries.FirstOrDefaultAsync(x => x.Path == file);
            if (entry != null)
            {
                context.PlayedEntries.Remove(entry);
            }
        }
        await context.SaveChangesAsync();
    }
}
