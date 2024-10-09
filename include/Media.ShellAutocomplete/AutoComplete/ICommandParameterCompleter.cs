// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.ShellAutoComplete.AutoComplete;

/// <summary>
/// Represents a command parameter completer.
/// </summary>
public interface ICommandCompletable
{
    /// <summary>
    /// Gets the suggestions for the specified parameter.
    /// </summary>
    /// <param name="parameter">Information on which parameter to get suggestions for.</param>
    /// <returns>The suggestions for the specified parameter.</returns>
    CompletionResult GetSuggestions(IMappedCommandParameter parameter, ICompletionContext context);
}