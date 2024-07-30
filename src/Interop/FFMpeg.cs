// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;

namespace Media.Interop;

internal sealed class FFMpeg
{
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

    private const string FfmpegBinary = "ffmpeg.exe";

    public static void EnsureIsInstalled()
    {
        if (!TryGetInstalledPath(out _))
        {
            throw new InvalidOperationException("ffmpeg not found.");
        }
    }

    public static bool TryGetInstalledPath(out string ffmpegPath)
    {
        ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FfmpegBinary);
        return File.Exists(ffmpegPath);
    }

    public static void StartFFMpeg(string commandLine)
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
