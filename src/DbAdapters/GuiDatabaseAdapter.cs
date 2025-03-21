// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;
using Media.Database.Entity;
using Media.Interop;
using Media.Ui.Gui;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

    public async Task<List<BookmarkViewModel>> GetBookmarks()
    {
        using var dbContext = GetContext();
        return await dbContext.FolderBookmarks.Select(e => new BookmarkViewModel
        {
            Path = e.Path,
            Name = e.Name,
        }).ToListAsync();
    }

    public async Task RemoveBookmark(BookmarkViewModel bookmark)
    {
        using var dbContext = GetContext();
        var entry = dbContext.FolderBookmarks.FirstOrDefault(e => e.Path == bookmark.Path);
        if (entry != null)
        {
            dbContext.FolderBookmarks.Remove(entry);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task AddBookMark(BookmarkViewModel folderBookmark)
    {
        using var dbContext = GetContext();
        var entry = dbContext.FolderBookmarks.FirstOrDefault(e => e.Path == folderBookmark.Path);
        if (entry == null)
        {
            dbContext.FolderBookmarks.Add(new FolderBookmark
            { 
                Name = folderBookmark.Name,
                Path = folderBookmark.Path,
            });
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<int> AddToLibary(IEnumerable<string> files, ILogger logger)
    {
        using var context = GetContext();
        foreach (var file in files)
        {
            if (MetadataFactory.TryCreateMetaData(file, out Metadata? created))
            {
                context.Metadata.Add(created);
            }
            else
            {
                logger.LogWarning("Failed to add {file} to library", file);
            }
        }
        return await context.SaveChangesAsync();
    }
}
