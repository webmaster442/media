// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;

namespace Media.Interop;

internal sealed class FFMpeg : IInterop
{
    private FFMpeg() { }

    public static class AudioCodecNames
    {
        public const string Alac = "alac";
        public const string PcmS16Le = "pcm_s16le";
        public const string Aac = "aac";
        public const string Copy = "copy";
        public const string Ac3 = "ac3";
    }

    public static class VideoCodecNames
    {
        public const string Copy = "copy";
    }

    public static class TargetNames
    {
        public const string NtscDvd = "ntsc-dvd";
        public const string PalDvd = "pal-dvd";
    }

    private const string FfmpegBinary = "ffmpeg.exe";

    public static void EnsureIsInstalled()
    {
        if (!TryGetInstalledPath(out _))
        {
            throw new InvalidOperationException("ffmpeg not found.");
        }
    }

    public static bool TryGetInstalledPath(out string toolPath)
    {
        toolPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FfmpegBinary);
        return File.Exists(toolPath);
    }

    public static void Start(string commandLine)
    {
        if (!TryGetInstalledPath(out string ffmpegPath))
        {
            throw new InvalidOperationException("FFMpeg not found.");
        }

        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = commandLine,
                UseShellExecute = false,
            }
        };

        process.Start();
    }
}
