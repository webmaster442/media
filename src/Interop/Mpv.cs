// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;

namespace Media.Interop;

internal sealed class Mpv : IInterop
{
    private Mpv() { }

    private const string MpvBinary = "mpv.exe";

    public static bool TryGetInstalledPath(out string toolPath)
    {
        toolPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MpvBinary);
        return File.Exists(toolPath);
    }

    public static void EnsureIsInstalled()
    {
        if (!TryGetInstalledPath(out _))
        {
            throw new InvalidOperationException("mpv not found.");
        }
    }

    public static void Start(string commandLine)
    {
        if (!TryGetInstalledPath(out string mpvPath))
        {
            throw new InvalidOperationException("mpv not found.");
        }

        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = mpvPath,
                Arguments = commandLine,
                UseShellExecute = false,
            }
        };

        process.Start();
    }
}
