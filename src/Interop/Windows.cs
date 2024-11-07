using System.Diagnostics;

namespace Media.Interop;

internal static class Windows
{
    public enum DisplaySwitchMode
    {
        Internal = 0,
        Clone = 1,
        Extended = 2,
        Exernal = 3,
    }

    public static void DisplaySwitch(DisplaySwitchMode mode)
    {
        static string Getargument(DisplaySwitchMode mode) => mode switch
        {
            DisplaySwitchMode.Internal => "/internal",
            DisplaySwitchMode.Clone => "/clone",
            DisplaySwitchMode.Extended => "/extend",
            DisplaySwitchMode.Exernal => "/external",
            _ => throw new UnreachableException(nameof(mode)),
        };

        var psi = new ProcessStartInfo
        {
            FileName = "DisplaySwitch.exe",
            Arguments = Getargument(mode),
            UseShellExecute = false,
            CreateNoWindow = false,
            
        };

        using var process = Process.Start(psi);
    }

    private static void ShudownCmd(string args)
    {
        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "shutdown.exe",
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
    }

    private static void RunDll32Cmd(string args)
    {
        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "rundll32.exe",
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
    }

    public static void Shutdown() => ShudownCmd("/s /t 0");

    public static void Restart() => ShudownCmd("/r /t 0");

    public static void Logoff() => ShudownCmd("/l /t 0");

    public static void Hibernate() => ShudownCmd("/h /t 0");

    public static void Sleep() => RunDll32Cmd("powrprof.dll,SetSuspendState 0,1,0");

    public static void Lock() => RunDll32Cmd("user32.dll,LockWorkStation");

    public static void ShellExecute(string file)
    {
        using var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = file,
                UseShellExecute = true,
            }
        };
        p.Start();
    }
}
