// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Spectre.Console.Cli;

namespace Media.Core.AutoComplete;

/// <summary>
/// Defines the context for command completion.
/// </summary>
public interface ICompletionContext
{
    /// <summary>
    /// Gets the elements of the command.
    /// </summary>
    string[] CommandElements { get; }

    /// <summary>
    /// Gets the partial element of the command.
    /// </summary>
    string PartialElement { get; }

    /// <summary>
    /// Gets the parent command container.
    /// </summary>
    ICommandContainer? Parent { get; }

    /// <summary>
    /// Gets the mapped parameters for the command.
    /// </summary>
    IReadOnlyCollection<IMappedCommandParameter> MappedParameters { get; }

    /// <summary>
    /// Gets the original command.
    /// </summary>

    IOriginalCommandInfo OriginalCommand { get; }
}