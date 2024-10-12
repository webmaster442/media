// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;

using Media.Interop;

using Spectre.Console;

namespace Media.Commands;

internal class DefaultCommand : Command
{
    private static void PrintReleaseNotes()
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Media.Notes.txt");
        if (stream == null)
            throw new InvalidOperationException("Can't find notes");

        using var reader = new StreamReader(stream);

        var content = reader.ReadToEnd();

        AnsiConsole.WriteLine(content);
    }

    [SupportedOSPlatform("windows")]
    private static bool WasItStartedByAShell()
    {
        [SupportedOSPlatform("windows")]
        static Process? GetParentProcess()
        {
            try
            {
                using var query = new System.Management.ManagementObjectSearcher($"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {Process.GetCurrentProcess().Id}");
                var results = query.Get().Cast<System.Management.ManagementObject>().FirstOrDefault();

                if (results != null)
                {
                    int parentId = Convert.ToInt32(results["ParentProcessId"]);
                    return Process.GetProcessById(parentId);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        var parent = GetParentProcess();

        HashSet<string> shells =
        [
            "cmd",
            "pwsh",
            "powershell",
            "wsl",
            "bash",
            "sh",
            "zsh",
            "fish"
        ];

        if (parent == null)
            return false;

        return shells.Contains(parent.ProcessName);
    }

    [SupportedOSPlatform("windows")]
    public override int Execute(CommandContext context)
    {
        bool shellStarted = WasItStartedByAShell();

        Powershell powershell = new();
        if (!shellStarted)
        {
            powershell.RunCommands(new string[]
            {
                $"\"{AppContext.BaseDirectory}\\Media.exe\" completion powershell | Out-String | Invoke-Expression",
                $"\"{AppContext.BaseDirectory}\\Media.exe\""
            });
        }
        else
        {
            PrintReleaseNotes();
        }

        return ExitCodes.Success;
    }
}
