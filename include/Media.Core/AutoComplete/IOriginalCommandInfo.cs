// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Core.AutoComplete;

/// <summary>
/// Represents a mapped command parameter.
/// </summary>
public interface IOriginalCommandInfo
{
    /// <summary>
    /// Gets the original command.
    /// </summary>
    string OriginalCommand { get; }

    /// <summary>
    /// Gets the original command.
    /// </summary>
    int? Position { get; }
}