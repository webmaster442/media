// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;

namespace Media.DbAdapters;

public sealed class ConfigAdapter
{
    private readonly DatabaseContext _databaseContext;

    public ConfigAdapter(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    private T? Read<T>(string key) where T : IParsable<T>
    {
        var rawValue = _databaseContext.Settings.Single(s => s.Key == key).Value;
        return T.Parse(rawValue, CultureInfo.InvariantCulture);
    }

    private string ReadString(string key)
        => _databaseContext.Settings.Single(s => s.Key == key).Value;

    private void WriteString(string key, string value)
    {
        var setting = _databaseContext.Settings.Single(s => s.Key == key);
        setting.Value = value;
    }

    public void Write<T>(string key, T value) where T : IFormattable, IParsable<T>
    {
        var setting = _databaseContext.Settings.Single(s => s.Key == key);
        setting.Value = value.ToString(null, CultureInfo.InvariantCulture);
    }

    public void WriteBool(string key, bool value)
    {
        var setting = _databaseContext.Settings.Single(s => s.Key == key);
        setting.Value = value.ToString(CultureInfo.InvariantCulture);
    }

    public DateTimeOffset FFMPegVesion
    {
        get => Read<DateTimeOffset>(ConfigKeys.FFMpegVersion);
        set => Write(ConfigKeys.FFMpegVersion, value);
    }

    public DateTimeOffset MpvVesion
    {
        get => Read<DateTimeOffset>(ConfigKeys.MpvVersion);
        set => Write(ConfigKeys.MpvVersion, value);
    }

    public DateTimeOffset YtdlpVesion
    {
        get => Read<DateTimeOffset>(ConfigKeys.YtdlpVersion);
        set => Write(ConfigKeys.YtdlpVersion, value);
    }

    public bool AlwaysOnTop
    {
        get => Read<bool>(ConfigKeys.AlwaysOnTop);
        set => WriteBool(ConfigKeys.AlwaysOnTop, value);
    }

    public bool ExitOnLaunch
    {
        get => Read<bool>(ConfigKeys.ExitOnLaunch);
        set => WriteBool(ConfigKeys.ExitOnLaunch, value);
    }

    public string ExternalFFMpegPath
    {
        get => ReadString(ConfigKeys.ExternalFfMpegPath);
        set => WriteString(ConfigKeys.ExternalFfMpegPath, value);
    }

    public string ExternalMpvPath
    {
        get => ReadString(ConfigKeys.ExternalMpvPath);
        set => WriteString(ConfigKeys.ExternalMpvPath, value);
    }

    public string ExternalYtdlpPath
    {
        get => ReadString(ConfigKeys.ExternalYtdlpPath);
        set => WriteString(ConfigKeys.ExternalYtdlpPath, value);
    }

    public int MpvRemotePort
    {
        get => Read<int>(ConfigKeys.MpvRemotePort);
        set => Write(ConfigKeys.MpvRemotePort, value);
    }

    public int DlnaServerPort
    {
        get => Read<int>(ConfigKeys.DlnaServerPort);
        set => Write(ConfigKeys.DlnaServerPort, value);
    }

    public void Save()
        => _databaseContext.SaveChanges();

    public async Task SaveAsync()
        => await _databaseContext.SaveChangesAsync();
}
