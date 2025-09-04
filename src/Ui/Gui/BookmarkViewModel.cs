// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Ui.Gui;

internal sealed class BookmarkViewModel
{
    public required string Path { get; init; }
    public required string Name { get; init; }

    public bool IsEnabled => Directory.Exists(Path);
}
