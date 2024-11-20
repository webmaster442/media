// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interop;

public sealed class ToolDependencyException : Exception
{
    public ToolDependencyException() : base()
    {
    }

    public ToolDependencyException(string? message) : base(message)
    {
    }

    public ToolDependencyException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
