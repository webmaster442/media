namespace Media.Dto;

public class ConfigObject
{
    public Dictionary<string, DateTimeOffset> Versions { get; }

    public (DateTimeOffset version, string[] encoders)? CachedHwEncoderList { get; set; }

    public ConfigObject()
    {
        Versions = new();
    }
}
