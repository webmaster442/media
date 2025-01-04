// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;
using Media.DbAdapters;
using Media.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Media;

internal static class ProgramFactory
{
    public static TypeRegistrar CreateTypeRegistar()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ConfigAdapter>();
        services.AddSingleton<ConfigAdapter>();
        services.AddSingleton<PlayedFilesAdapter>();
        services.AddSingleton<ApiCacheAdapter>();
        services.AddSingleton<RadioStationsClient>();
        services.AddSingleton<GuiDatabaseAdapter>();
        var registar = new TypeRegistrar(services);
        registar.Build();
        return registar;
    }

    public static ILoggerFactory GetLoggerFactory()
    {
        return LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.AddConsole();
            builder.AddFilter(loglevel => loglevel >= LogLevel.Information);
        });
    }

    public static async Task RunDatabaseJobsIfNeeded()
    {
        using var db = new DatabaseContext();
        db.RunMigrationsIfNeeded();

        using var logger = GetLoggerFactory();

        var jobRunner = new DatabaseJobRunner(db, DateTime.Now, logger.CreateLogger("Database jobs"));

        if (jobRunner.IsAnyJobToRun())
        {
            bool isAborted = await Terminal.AbortCountdown("Preparing to run database jobs.", 5);
            if (!isAborted)
            {
                await jobRunner.RunJobs(false);
            }
        }
    }
}
