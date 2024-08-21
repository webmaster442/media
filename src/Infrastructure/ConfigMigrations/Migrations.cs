using Media.Dto.Config;

namespace Media.Infrastructure.ConfigMigrations;

public class Migrations
{
    private IEnumerable<IMigrator> AvailableMigrations
    {
        get
        {
            yield break;
        }
    }

    public void ApplyMigrations(ConfigObject configObject)
    {
        var toApply = AvailableMigrations
            .Where(m => m.Version > configObject.Version)
            .OrderBy(m => m.Version);

        foreach (var migration in toApply)
        {
            migration.Migrate(configObject.Settings);
            configObject.Version = migration.Version;
        }
    }
}
