// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;
using Media.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddLogging();
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
        DirectoryInfo di = new DirectoryInfo(AppContext.BaseDirectory);
        var acl = di.GetAccessControl(AccessControlSections.All);
        foreach (AuthorizationRule rule in acl.GetAccessRules(true, true, typeof(NTAccount)))
        {
            if (rule.IdentityReference.Value.Equals(Environment.UserName, StringComparison.CurrentCultureIgnoreCase)
                && rule is FileSystemAccessRule filesystemAccessRule
                && (filesystemAccessRule.FileSystemRights & FileSystemRights.WriteData) > 0
                && filesystemAccessRule.AccessControlType != AccessControlType.Deny)
            {
                return true;
            }
        }
        return false;
    }
}
