// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto.Config;

public static class ConfigKeys
{
    public const string FFMpegVersion = "FFMpegVersion";
    public const string MpvVersion = "MpvVersion";
    public const string YtdlpVersion = "YtdlpVersion";
    public const string ExternalFfMpegPath = "ExternalFfMpegPath";
    public const string ExternalMpvPath = "ExternalMpvPath";
    public const string ExternalYtdlpPath = "ExternalYtdlpPath";
    public const string MpvRemotePort = "MpvRemotePort";
    public const string DlnaServerPort = "DlnaServerPort";

    public static readonly Dictionary<string, string> CurrentVersionDefaults = new()
    {
        { FFMpegVersion, new DateTimeOffset(DateTime.MinValue).ToString() },
        { MpvVersion, new DateTimeOffset(DateTime.MinValue).ToString() },
        { YtdlpVersion, new DateTimeOffset(DateTime.MinValue).ToString() },
        { ExternalFfMpegPath, string.Empty },
        { ExternalMpvPath, string.Empty },
        { ExternalYtdlpPath, string.Empty },
        { MpvRemotePort, 12345.ToString() },
        { DlnaServerPort, 8085.ToString() }
    };
}
