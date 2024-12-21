// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Database.Entity;

public sealed class ApiCacheEntry
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required double ValidityInSeconds { get; set; }
}
