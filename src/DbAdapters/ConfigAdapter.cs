// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;

namespace Media.DbAdapters;

internal sealed class ConfigAdapter : DatabaseAdapterBase
{
    private T? Read<T>(string key) where T : IParsable<T>
    {
        using var databaseContext = GetContext();
        var rawValue = databaseContext.Settings.Single(s => s.Key == key).Value;
        return T.Parse(rawValue, CultureInfo.InvariantCulture);
    }

    private string ReadString(string key)
    {
        using var databaseContext = GetContext();
        return databaseContext.Settings.Single(s => s.Key == key).Value;
    }

    private void WriteString(string key, string value)
    {
        using var databaseContext = GetContext();
        var setting = databaseContext.Settings.Single(s => s.Key == key);
        setting.Value = value;
        databaseContext.SaveChanges();
    }

    public void Write<T>(string key, T value) where T : IFormattable, IParsable<T>
    {
        using var databaseContext = GetContext();
        var setting = databaseContext.Settings.Single(s => s.Key == key);
        setting.Value = value.ToString(null, CultureInfo.InvariantCulture);
        databaseContext.SaveChanges();
    }

    public void WriteBool(string key, bool value)
    {
        using var databaseContext = GetContext();
        var setting = databaseContext.Settings.Single(s => s.Key == key);
        setting.Value = value.ToString(CultureInfo.InvariantCulture);
        databaseContext.SaveChanges();
    }

    [Description("Lastly installed FFMpeg Build Date")]
    public DateTimeOffset FFMPegVesion
    {
        get => Read<DateTimeOffset>(ConfigKeys.FFMpegVersion);
        set => Write(ConfigKeys.FFMpegVersion, value);
    }

    [Description("Lastly installed MPV Build Date")]
    public DateTimeOffset MpvVesion
    {
        get => Read<DateTimeOffset>(ConfigKeys.MpvVersion);
        set => Write(ConfigKeys.MpvVersion, value);
    }

    [Description("Lastly installed Yt-dlp Build Date")]
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

    [Description("Specifies path for FFMpeg to use instead of update installed")]
    public string ExternalFFMpegPath
    {
        get => ReadString(ConfigKeys.ExternalFfMpegPath);
        set => WriteString(ConfigKeys.ExternalFfMpegPath, value);
    }

    [Description("Specifies path for MPV to use instead of update installed")]
    public string ExternalMpvPath
    {
        get => ReadString(ConfigKeys.ExternalMpvPath);
        set => WriteString(ConfigKeys.ExternalMpvPath, value);
    }

    [Description("Specifies path for Yt-dlp to use instead of update installed")]
    public string ExternalYtdlpPath
    {
        get => ReadString(ConfigKeys.ExternalYtdlpPath);
        set => WriteString(ConfigKeys.ExternalYtdlpPath, value);
    }

    [Description("MPV http remote port to use")]
    public int MpvRemotePort
    {
        get => Read<int>(ConfigKeys.MpvRemotePort);
        set => Write(ConfigKeys.MpvRemotePort, value);
    }

    [Description("DLNA Server port to use")]
    public int DlnaServerPort
    {
        get => Read<int>(ConfigKeys.DlnaServerPort);
        set => Write(ConfigKeys.DlnaServerPort, value);
    }
}
