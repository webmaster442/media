// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;
using Media.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Security.AccessControl;
using System.Security.Principal;

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

        services.AddLogging(log => {
            log.AddConsole();
            log.AddFilter((category, level) => level > LogLevel.Debug && !category.StartsWith("Microsoft.EntityFrameworkCore"));
        });
        services.AddSingleton<ConfigAccessor>();
        services.AddSingleton<IDryRunResultAcceptor>(dryRunResultAcceptor);
        services.AddDbContext<MediaDatabaseContext>(options => options.UseSqlite("Data Source=media.db"));
        services.AddScoped<MediaDbSerives>();
        var registar = new TypeRegistrar(services);
        registar.Build();
        return registar;
    }

    public static bool CanWriteAppFolder()
    {
        try
        {
            var appDirectory = new DirectoryInfo(AppContext.BaseDirectory);
            var directorySecurity = appDirectory.GetAccessControl();
            var currentUser = WindowsIdentity.GetCurrent();
            var currentPrincipal = new WindowsPrincipal(currentUser);

            foreach (FileSystemAccessRule rule in directorySecurity.GetAccessRules(includeExplicit: true,
                                                                                   includeInherited: true,
                                                                                   targetType: typeof(SecurityIdentifier)))
            {
                if ((currentUser.User?.Equals(rule.IdentityReference) == true
                    || currentPrincipal.IsInRole((SecurityIdentifier)rule.IdentityReference))
                    && (rule.FileSystemRights & FileSystemRights.WriteData) != 0)
                {
                    return rule.AccessControlType != AccessControlType.Deny;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
