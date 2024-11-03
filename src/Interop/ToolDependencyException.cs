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
