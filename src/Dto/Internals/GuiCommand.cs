// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto.Internals;

internal sealed class GuiCommand
{
    public required string Description { get; init; }
    public required string CommandLine { get; init; }
    public required string ButtonText { get; init; }
    public required GuiCommandPart[] Editors { get; init; }
}
