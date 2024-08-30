// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Config;

namespace Media.Infrastructure;

public sealed class ConfigAccessor
{
    private readonly ConfigObject _config;
    private readonly JsonSerializerOptions _options;

    private T? Read<T>(string key, T? defaultValue = default) where T : IParsable<T>
    {
        if (_config.Settings.TryGetValue(key, out string? value)
            && T.TryParse(value, CultureInfo.InvariantCulture, out T? parsed))
        {
            return parsed;
        }
        return defaultValue;
    }

    private void Write<T>(string key, T value) where T : IFormattable, IParsable<T>
    {
        _config.Settings[key] = value.ToString(null, CultureInfo.InvariantCulture);
    }

    private ConfigObject LoadConfig()
    {
        using var stream = File.OpenRead(ConfigPath);
        var loaded = JsonSerializer.Deserialize<ConfigObject>(stream, _options)
            ?? throw new InvalidOperationException("Config file deserialization error");

        var migrator = new ConfigMigrations.Migrations();
        migrator.ApplyMigrations(loaded);

        return loaded;
    }

    private async Task SaveConfig()
    {
        var temp = Path.GetTempFileName();
        await using (var stream = File.Create(temp))
        {
            await JsonSerializer.SerializeAsync(stream, _config, _options);
        }
        File.Move(temp, ConfigPath, true);
        File.Delete(temp);
    }

    public ConfigAccessor()
    {
        ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.config.json");
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        if (File.Exists(ConfigPath))
        {
            _config = LoadConfig();
        }
        else
        {
            _config = new ConfigObject();
            _config.FillWithDefaults();
        }
    }

    public string ConfigPath { get; }

    public Task ForceSave() => SaveConfig();

    public DateTimeOffset? GetFFMPegVesion()
        => Read<DateTimeOffset>(ConfigKeys.FFMpegVersion);

    public async Task SetFFMpegVersion(DateTimeOffset publishedAt)
    {
        Write(ConfigKeys.FFMpegVersion, publishedAt);
        await SaveConfig();
    }

    public DateTimeOffset? GetMpvVesion()
        => Read<DateTimeOffset>(ConfigKeys.MpvVersion);

    public async Task SetMpvVersion(DateTimeOffset publishedAt)
    {
        Write(ConfigKeys.MpvVersion, publishedAt);
        await SaveConfig();
    }

    public DateTimeOffset? GetYtdlpVesion()
        => Read<DateTimeOffset>(ConfigKeys.YtdlpVersion);

    public async Task SetYtdlpVersion(DateTimeOffset publishedAt)
    {
        Write(ConfigKeys.YtdlpVersion, publishedAt);
        await SaveConfig();
    }

    public string? GetExternalFFMpegPath()
        => Read<string>(ConfigKeys.ExternalFfMpegPath);

    public string? GetExternalMpvPath()
        => Read<string>(ConfigKeys.ExternalMpvPath);

    public string? GetExternalYtdlpPath()
        => Read<string>(ConfigKeys.ExternalYtdlpPath);

    public int? GetMpvRemotePort()
        => Read<int>(ConfigKeys.MpvRemotePort);

    public int? GetDlnaServerPort()
        => Read<int>(ConfigKeys.DlnaServerPort);
}
