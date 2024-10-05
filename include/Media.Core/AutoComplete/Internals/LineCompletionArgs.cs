// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Core.AutoComplete.Internals;

internal class TabCompletionArgs
{
    public string Command { get; set; }
    public int? CursorPosition { get; set; }

    public TabCompletionArgs(string command, int? cursorPosition = null)
    {
        Command = command;
        CursorPosition = cursorPosition;
    }
}