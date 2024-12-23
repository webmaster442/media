// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database.Entity;

using Microsoft.EntityFrameworkCore;

namespace Media.DbAdapters;

internal sealed class PlayedFilesAdapter : DatabaseAdapterBase
{
    public async Task<HashSet<string>> GetPlayedFilesAsync()
    {
        using var dbContext = GetContext();
        return await dbContext.PlayedEntries
            .Select(x => x.Path)
            .ToHashSetAsync();
    }

    public async Task AddPlayedFilesAsync(IEnumerable<string> files)
    {
        using var dbContext = GetContext();
        foreach (var file in files)
        {
            var entry = await dbContext.PlayedEntries.FindAsync(file);
            if (entry is not null)
            {
                entry.LastPlayed = DateTime.Now;
            }
            else
            {
                await dbContext.PlayedEntries.AddAsync(new PlayedEntry
                {
                    Path = file,
                    LastPlayed = DateTime.Now
                });
            }
        }
        await dbContext.SaveChangesAsync();
    }

}
