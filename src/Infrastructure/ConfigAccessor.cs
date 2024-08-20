using Media.Dto;

namespace Media.Infrastructure;

public sealed class ConfigAccessor
{
    private readonly ConfigObject _config;
    private readonly JsonSerializerOptions _options;
    private readonly string _configPath;
    public ConfigAccessor()
    {
        _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.config.json");
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        _config = File.Exists(_configPath) ? LoadConfig() : new ConfigObject();
    }

    private ConfigObject LoadConfig()
    {
        using var stream = File.OpenRead(_configPath);
        return JsonSerializer.Deserialize<ConfigObject>(stream, _options)
            ?? throw new InvalidOperationException("Config file deserialization error");
    }

    private async Task SaveConfig()
    {
        var temp = Path.GetTempFileName();
        await using (var stream = File.Create(temp))
        {
            await JsonSerializer.SerializeAsync(stream, _config, _options);
        }
        File.Move(temp, _configPath, true);
        File.Delete(temp);
    }

    public DateTimeOffset? GetInstalledVersion(string programName)
    {
        if (_config.Versions.TryGetValue(programName, out DateTimeOffset version))
        {
            return version;
        }
        return null;
    }

    public async Task SetInstalledVersion(string programName, DateTimeOffset publishedAt)
    {
        _config.Versions[programName] = publishedAt;
        await SaveConfig();
    }

    public (DateTimeOffset Version, string[] encoders)? GetCachedEncoderList()
    {
        return _config.CachedHwEncoderList;
    }

    public async Task SetCachedEncoderList(DateTimeOffset version, string[] encoderList)
    {
        _config.CachedHwEncoderList = (version, encoderList);
        await SaveConfig();
    }
}
