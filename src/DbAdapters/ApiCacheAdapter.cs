// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database.Entity;
using Media.Dto.Internals;

using Microsoft.EntityFrameworkCore;

namespace Media.DbAdapters;

internal sealed class ApiCacheAdapter : DatabaseAdapterBase
{
    public const string RadioCountries = "RadioCountries";
    public const string StationBase = "RadioStation_";

    public const double DefaultValidity = 24.0 * 60 * 60;

    public async Task<CacheEntry?> GetEntry(string key)
    {
        using var dbContext = GetContext();

        var entity = await dbContext.ApiCacheEntries
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
        using var dbContext = GetContext();

        var entity = await dbContext.ApiCacheEntries
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
            dbContext.ApiCacheEntries.Add(entity);
        }
        else
        {
            entity.CreatedAt = curentDate;
            entity.ValidityInSeconds = validityInSeconds;
            entity.Value = value;
        }
        await dbContext.SaveChangesAsync();
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