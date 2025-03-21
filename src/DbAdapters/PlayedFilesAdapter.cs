// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
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

            Metadata? metadata = await dbContext.Metadata.FindAsync(file);
            if (metadata is null && MetadataFactory.TryCreateMetaData(file, out Metadata? created))
            {
                await dbContext.Metadata.AddAsync(created);
            }
        }
        await dbContext.SaveChangesAsync();
    }
}
