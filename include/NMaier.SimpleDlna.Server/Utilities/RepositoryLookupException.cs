namespace NMaier.SimpleDlna.Server.Utilities;

[Serializable]
public sealed class RepositoryLookupException : ArgumentException
{
    public RepositoryLookupException(string key)
      : base($"Failed to lookup {key}")
    {
        Key = key;
    }

    public string Key { get; private set; } = string.Empty;

    public RepositoryLookupException()
    {
    }

    public RepositoryLookupException(string message, Exception innerException) : base(message, innerException)
    {
    }
}