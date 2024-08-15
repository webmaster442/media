// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure.Selector;

namespace Media.Dto.Internals;

internal sealed class Item : IITem
{
    public required string Name { get; init; }
    public required string FullPath { get; init; }
    public required string Icon { get; init; }
}
