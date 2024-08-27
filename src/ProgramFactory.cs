// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace Media;

internal static class ProgramFactory
{
    public static TypeRegistrar CreateTypeRegistar(bool isDryRunEnabled)
    {
        return CreateTypeRegistar(new DryRunResultAcceptor
        {
            Enabled = isDryRunEnabled
        });
    }

    public static TypeRegistrar CreateTypeRegistar(IDryRunResultAcceptor dryRunResultAcceptor)
    {
        var services = new ServiceCollection();

        services.AddSingleton<ConfigAccessor>();
        services.AddSingleton<IDryRunResultAcceptor>(dryRunResultAcceptor);
        var registar = new TypeRegistrar(services);
        registar.Build();
        return registar;
    }
}
