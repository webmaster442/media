﻿using System.Diagnostics;
using System.Text.Json;

namespace FFCmd.FFMpegInterop;

internal sealed class FFMpeg
{
    private const string ffmpegBinary = "ffmpeg.exe";
    private const string ffmpegVersionFile = "ffmpeg.json";

    private static bool TryGetInstalledPath(out string ffmpegPath)
    {
        ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ffmpegBinary);
        return File.Exists(ffmpegPath);
    }

    internal static async Task SetInstalledVersion(DateTimeOffset? publishedAt)
    {
        var versionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ffmpegVersionFile);
        using var stream = File.Create(versionFile);
        await JsonSerializer.SerializeAsync(stream, publishedAt);
    }

    internal static async Task<DateTimeOffset?> GetInstalledVersion()
    {
        var versionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ffmpegVersionFile);
        if (!File.Exists(versionFile))
        {
            return null;
        }
        using var stream = File.OpenRead(versionFile);
        return await JsonSerializer.DeserializeAsync<DateTimeOffset>(stream);
    }

    public static void StartFFMpeg(FFMpegCommandBuilder command)
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
                Arguments = command.BuildCommandLine(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        process.Start();
    }
}
