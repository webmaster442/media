namespace NMaier.SimpleDlna.Server.Types;

[Serializable]
public class HttpException : Exception
{
    public HttpException()
    {
    }

    public HttpException(string message)
      : base(message)
    {
    }

    public HttpException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
}