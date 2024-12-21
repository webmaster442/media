// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;
using Media.DbAdapters;
using Media.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace Media;

internal static class ProgramFactory
{
    public static TypeRegistrar CreateTypeRegistar()
    {
        // TypeRegistar will dispose the db object
        // Dispose objects before losing scope
#pragma warning disable CA2000
        var db = new DatabaseContext();
#pragma warning restore CA2000
        db.RunMigrations();

        var services = new ServiceCollection();
        services.AddSingleton(db);
        services.AddSingleton<ConfigAccessor>();
        services.AddSingleton<ConfigAccessor>();
        services.AddSingleton<PlayedFilesAdapter>();
        services.AddSingleton<ApiCacheAdapter>();
        services.AddSingleton<RadioStationsClient>();
        var registar = new TypeRegistrar(services);
        registar.Build();
        return registar;
    }
}
