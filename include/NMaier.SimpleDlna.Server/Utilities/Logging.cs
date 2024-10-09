using Microsoft.Extensions.Logging;

namespace NMaier.SimpleDlna.Server.Utilities;

public class Logging
{
    public Logging(ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger(GetType());
        LoggerFactory = loggerFactory;
    }

    public ILogger Logger { get; }
    protected ILoggerFactory LoggerFactory { get; }
}