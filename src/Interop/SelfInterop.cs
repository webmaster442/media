using System.Diagnostics;
using System.Runtime.InteropServices;

using Media.Infrastructure;
using Media.Interop.CdRip;

namespace Media.Interop;

internal static class SelfInterop
{
    private static void BringConsoleWindowToFront()
    {
        IntPtr handle = Win32Functions.FindWindow(null, Console.Title);

        if (handle == IntPtr.Zero)
        {
            //Console window not found!
            return;
        }

        Win32Functions.ShowWindow(handle, Win32Functions.SW_RESTORE);
        Win32Functions.SetForegroundWindow(handle);
    }

    private static void RunMedia(string[] args)
    {
        BringConsoleWindowToFront();
        using var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(AppContext.BaseDirectory, "media.exe"),
                UseShellExecute = false
            }
        };
        p.StartInfo.ArgumentList.AddRange(args);
        p.Start();
    }

    public static void RunMediaCommand(params string[] cmd)
        => RunMedia(cmd);

    public static void Play(string file)
        => RunMediaCommand("play", "file", file, "-r");
}
