// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interop;

internal static class MpvIpcCommandFactory
{
    public static string[] SeekRelative(double seconds)
        => ["seek", "relative", seconds.ToString(CultureInfo.InvariantCulture)];

    public static string[] SeekAbsolute(double seconds)
        => ["seek", "absolute", seconds.ToString(CultureInfo.InvariantCulture)];

    public static string[] PlayListNext()
        => ["playlist-next"];

    public static string[] PlayListPrevious()
        => ["playlist-prev"];

    public static string[] Quit()
        => ["quit", "0"];

    public static string[] QuitSavePos()
        => ["quit-watch-later", "0"];

    public static string[] Play()
        => ["set_property", "pause", "false"];

    public static string[] Pause()
        => ["set_property", "pause", "true"];

    public static string[] GetPosition()
        => ["get_property", "time-pos"];

    public static string[] CycleSubtitle()
        => ["cycle", "sub"];

    public static string[] CycleAudio()
        => ["cycle", "audio"];
}
