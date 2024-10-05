// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Core.AutoComplete;

/// <summary>
/// Represents a command parameter completer.
/// </summary>
public interface IAsyncCommandCompletable
{
    /// <summary>
    /// Gets the suggestions for the specified parameter.
    /// </summary>
    /// <param name="parameter">Information on which parameter to get suggestions for.</param>
    /// <returns>The suggestions for the specified parameter.</returns>
    //Task<CompletionResult> GetSuggestionsAsync(ICommandParameterInfo parameter, string? prefix);
    Task<CompletionResult> GetSuggestionsAsync(IMappedCommandParameter parameter, ICompletionContext context);
}