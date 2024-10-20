// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto.Internals;

internal class DriveData
{
    public required string Name { get; init; }
    public required string Label { get; init; }
    public required string Format { get; init; }
    public required string Type { get; init; }
    public required string TotalHumanSize { get; init; }
    public required string AvailableHumanSize { get; init; }
    public required string PercentUsed { get; init; }
}
