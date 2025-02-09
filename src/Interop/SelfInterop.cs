// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

using Media.Infrastructure;

using Spectre.Console;

namespace Media.Interop;

internal static class SelfInterop
{
    public static string CurentProgramPath
        => Path.Combine(AppContext.BaseDirectory, "media.exe");

    private static void RunMedia(string[] args)
    {
        AnsiConsole.MarkupLine("[green]Executing[/] {0}...", string.Join(' ', args).EscapeMarkup());
        using var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = CurentProgramPath,
                UseShellExecute = false
            }
        };
        p.StartInfo.AddArguments(args);
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
        => RunMediaCommand("play", "file", file);
}
