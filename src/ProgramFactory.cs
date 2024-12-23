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
}
