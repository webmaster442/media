using Microsoft.Extensions.Logging;

namespace Media.Database;

internal interface IDatabaseJob
{
    public TimeSpan TriggerInterval { get; }
    Task RunJob(DatabaseContext databaseContext, ILogger logger);
}
