// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto.Internals;

internal class CacheEntry
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public required DateTime ValidEndDate { get; set; }
}
