namespace Media.Infrastructure.ConfigMigrations;

public interface IMigrator
{
    Version Version { get; }
    void Migrate(IDictionary<string, string> keyValuePairs);
}
