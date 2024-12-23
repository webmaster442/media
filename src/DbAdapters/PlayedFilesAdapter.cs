// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows.Xps.Packaging;

using Media.Database;
using Media.Database.Entity;

using Microsoft.EntityFrameworkCore;

namespace Media.DbAdapters;

internal sealed class PlayedFilesAdapter
{
    private readonly DatabaseContext _dbContext;

    public PlayedFilesAdapter(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HashSet<string>> GetPlayedFilesAsync()
    {
        return await _dbContext.PlayedEntries
            .Select(x => x.Path)
            .ToHashSetAsync();
    }

    public async Task AddPlayedFilesAsync(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            var entry = await _dbContext.PlayedEntries.FindAsync(file);
            if (entry is not null)
            {
                entry.LastPlayed = DateTime.Now;
            }
            else
            {
                await _dbContext.PlayedEntries.AddAsync(new PlayedEntry
                {
                    Path = file,
                    LastPlayed = DateTime.Now
                });
            }
        }
        await _dbContext.SaveChangesAsync();
    }

}
