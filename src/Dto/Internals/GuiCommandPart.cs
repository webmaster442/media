// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto.Internals;

internal class GuiCommandPart
{
    public required string Name { get; init; }
    public required GuiCommandPartEditor Editor { get; init; }
    public required string Description { get; init; }
}
