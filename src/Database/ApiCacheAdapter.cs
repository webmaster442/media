// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database.Entity;
using Media.Dto.Internals;

using Microsoft.EntityFrameworkCore;

namespace Media.Database;

internal sealed class ApiCacheAdapter
{
    private readonly DatabaseContext _dbContext;

    public ApiCacheAdapter(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public const string RadioCountries = "RadioCountries";
    public const string StationBase = "RadioStation_";

    public const double DefaultValidity = (24.0 * 60 * 60);

    public async Task<CacheEntry?> GetEntry(string key)
    {
        var entity = await _dbContext.ApiCacheEntries
            .Where(x => x.Key == key)
            .FirstOrDefaultAsync();

        if (entity is not null)
        {
            return new CacheEntry
            {
                Key = entity.Key,
                Value = entity.Value,
                ValidEndDate = entity.CreatedAt.AddSeconds(entity.ValidityInSeconds)
            };
        }

        return null;
    }

    public async Task SetEntry(string key, string value, DateTime curentDate, double validityInSeconds = DefaultValidity)
    {
        var entity = await _dbContext.ApiCacheEntries
            .Where(x => x.Key == key)
            .FirstOrDefaultAsync();

        if (entity is null)
        {
            entity = new ApiCacheEntry
            {
                Key = key,
                CreatedAt = curentDate,
                ValidityInSeconds = validityInSeconds,
                Value = value
            };
            _dbContext.ApiCacheEntries.Add(entity);
        }
        else
        {
            entity.CreatedAt = curentDate;
            entity.ValidityInSeconds = validityInSeconds;
            entity.Value = value;
        }
        await _dbContext.SaveChangesAsync();
    }
}

internal static class CacheEntryExtensions
{
    public static bool IsValid(this CacheEntry entry, DateTime currentTime)
    {
        return !string.IsNullOrWhiteSpace(entry.Value)
            && currentTime <= entry.ValidEndDate;
    }

    public static T Deserialize<T>(this CacheEntry entry)
    {
        return JsonSerializer.Deserialize<T>(entry.Value)
            ?? throw new InvalidOperationException("Deserialize failed");
    }
}