using System.Globalization;

using Media.Database.Entity;

using Microsoft.Extensions.Logging;

namespace Media.Database;

public class DatabaseJobRunner
{
    private readonly List<IDatabaseJob> _jobs;
    private readonly DatabaseContext _databaseContext;
    private readonly DateTime _currentDate;
    private readonly DateTime _lastCleanupDate;
    private readonly ILogger _logger;

    public DatabaseJobRunner(DatabaseContext databaseContext, DateTime currentDate, ILogger logger)
    {
        _databaseContext = databaseContext;
        _currentDate = currentDate;
        _logger = logger;
        _lastCleanupDate = GetLastCleanupDate();
        _jobs = LoadJobs();
    }

    private DateTime GetLastCleanupDate()
    {
        var value = _databaseContext.Settings.FirstOrDefault(s => s.Key == ConfigKeys.LastCleanupDate)?.Value;
        if (string.IsNullOrEmpty(value))
            return DateTime.MinValue;

        if (DateTime.TryParse(value, out DateTime result))
        {
            return result;
        }

        return DateTime.MinValue;
    }

    private void UpdateLastCleanupDate()
    {
        var entry = _databaseContext.Settings.FirstOrDefault(s => s.Key == ConfigKeys.LastCleanupDate);
        if (entry != null)
        {
            entry.Value = _currentDate.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            _databaseContext.Settings.Add(new Setting
            {
                Key = ConfigKeys.LastCleanupDate,
                Value = _currentDate.ToString(CultureInfo.InvariantCulture)
            });
        }
        _databaseContext.SaveChanges();
    }

    private List<IDatabaseJob> LoadJobs()
    {
        List<IDatabaseJob> jobs = new();

        var jobTypes = typeof(DatabaseJobRunner).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IDatabaseJob)));

        foreach (var jobType in jobTypes)
        {
            try
            {
                if (Activator.CreateInstance(jobType) is IDatabaseJob job)
                {
                    jobs.Add(job);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load job: {jobName}", jobType.Name);
            }
        }

        return jobs;
    }

    private bool NeedsRunning(IDatabaseJob job)
        => (_currentDate - job.TriggerInterval) >= _lastCleanupDate;

    public bool IsAnyJobToRun()
        => _jobs.Any(NeedsRunning);

    public async Task RunJobs(bool force = false)
    {
        IEnumerable<IDatabaseJob> jobToRun = Enumerable.Empty<IDatabaseJob>();

        jobToRun = force
            ? _jobs
            : _jobs.Where(NeedsRunning);

        foreach (var job in jobToRun)
        {
            await job.RunJob(_databaseContext, _logger);
        }

        UpdateLastCleanupDate();
    }
}
