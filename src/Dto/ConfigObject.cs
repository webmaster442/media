namespace Media.Dto;

public class ConfigObject
{
    public Dictionary<string, DateTimeOffset> Versions { get; }

    public ConfigObject()
    {
        Versions = new();
    }
}
