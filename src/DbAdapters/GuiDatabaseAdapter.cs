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
}
