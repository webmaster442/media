namespace Media.Dto.Config;

public sealed class ConfigObject
{
    public Dictionary<string, DateTimeOffset> Versions { get; }

    public EncoderInfos? EncoderInfoCache { get; set; }

    public ConfigObject()
    {
        Versions = new();
    }
}
