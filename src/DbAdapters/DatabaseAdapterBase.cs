// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;

namespace Media.DbAdapters;

internal class DatabaseAdapterBase
{
    protected DatabaseContext GetContext()
    {
        var db = new DatabaseContext();
        db.RunMigrationsIfNeeded();
        return db;
    }

    public async Task ForceDbJobRunning()
    {
        await using var context = GetContext();
        using var logger = ProgramFactory.GetLoggerFactory();

        var jobRunner = new DatabaseJobRunner(context, DateTime.Now, logger.CreateLogger("Database jobs"));

        await jobRunner.RunJobs(true);
    }
}
