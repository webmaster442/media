// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database.Entity;

using Microsoft.EntityFrameworkCore;

namespace Media.Database;

internal sealed class MediaDocumentStoreAdapter
{
    private readonly DatabaseContext _dbContext;

    public MediaDocumentStoreAdapter(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HashSet<string>> GetPlayedFilesAsync()
    {
        return await _dbContext.PlayedEntries
            .Select(x => x.Path)
            .ToHashSetAsync();
    }

    public async Task AddPlayedFileAsync(IEnumerable<string> files)
    {
        var entries = files.Select(file => new PlayedEntry
        {
            Path = file, 
            LastPlayed = DateTime.Now
        });
        await _dbContext.PlayedEntries.AddRangeAsync(entries);
        await _dbContext.SaveChangesAsync();
    }

}
