namespace AudioSwitcher.CoreAudio.Threading;

[Serializable]
public sealed class InvalidThreadException : Exception
{
    public InvalidThreadException(string message)
        : base(message)
    {
    }

    public InvalidThreadException()
    {
    }

    public InvalidThreadException(string message, Exception innerException) : base(message, innerException)
    {
    }
}