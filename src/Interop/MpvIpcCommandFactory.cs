// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------


namespace Media.Interop;

internal static class MpvIpcCommandFactory
{
    public static string[] SeekRelative(double seconds)
        => ["seek", seconds.ToString(CultureInfo.InvariantCulture), "relative"];

    public static string[] PlayListNext()
        => ["playlist-next"];

    public static string[] PlayListPrevious()
        => ["playlist-prev"];

    public static string[] Play()
        => ["set_property", "pause", "no"];

    public static string[] Pause()
        => ["set_property", "pause", "yes"];

    public static string[] CycleSubtitle()
        => ["cycle", "sub"];

    public static string[] CycleAudio()
        => ["cycle", "audio"];
    public static string[] Fullscreen(bool enabled)
        => ["set_property", "fullscreen", enabled ? "yes" : "no"];

    public static string[] Mute(bool enabled)
        => ["set_property", "mute", enabled ? "yes" : "no"];

    public static string[] VolumeRelative(double value)
        => ["add", "volume", value.ToString(CultureInfo.InvariantCulture)];
}
