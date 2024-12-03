// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;

using Media.Infrastructure;

using Spectre.Console;

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
        AnsiConsole.MarkupLine("[green]Executing[/] {0}...", string.Join(' ', args));
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

    public static void StartShell(string path)
    {
        Powershell powershell = new(updatePathVar: true);
        powershell.RunCommands(new string[]
        {
            $"\"{AppContext.BaseDirectory}\\Media.exe\" completion powershell | Out-String | Invoke-Expression",
            $"pushd '{path}'"
        }, shellExecute: true);
    }

    public static void RunMediaCommand(params string[] cmd)
        => RunMedia(cmd);

    public static void Play(string file)
        => RunMediaCommand("play", "file", file, "-r");
}
