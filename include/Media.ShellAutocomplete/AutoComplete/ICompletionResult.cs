// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.ShellAutoComplete.AutoComplete;

/// <summary>
/// Represents a completion result.
/// </summary>
public interface ICompletionResult
{
    /// <summary>
    /// Gets a value indicating whether or not automatic completions should be disabled.
    /// </summary>
    bool PreventDefault { get; }

    /// <summary>
    /// Gets the suggestions.
    /// </summary>
    IEnumerable<CompletionResultItem> Suggestions { get; }
}