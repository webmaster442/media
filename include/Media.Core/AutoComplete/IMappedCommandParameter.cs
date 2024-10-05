// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Spectre.Console.Cli;

namespace Media.Core.AutoComplete;

/// <summary>
/// Defines a mapped command parameter.
/// </summary>
public interface IMappedCommandParameter
{
    /// <summary>
    /// Gets the parameter information for the command.
    /// </summary>
    ICommandParameterInfo Parameter { get; }

    /// <summary>
    /// Gets the value of the command parameter.
    /// </summary>
    string? Value { get; }
}