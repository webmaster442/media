// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;

using Media.Infrastructure;

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

    private void StartInPowerShell()
    {
        static string? FindPowershellCore()
        {
            string? pathVariable = Environment.GetEnvironmentVariable("PATH");
            if (pathVariable != null)
            {
                string[] paths = pathVariable.Split(';');
                foreach (string path in paths)
                {
                    string pwshPath = Path.Combine(path, "pwsh.exe");
                    if (File.Exists(pwshPath))
                    {
                        return pwshPath;
                    }
                }
            }
            return null;
        }

        var powershell = FindPowershellCore() ?? "powershell.exe";
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = powershell,
                Arguments = $"-NoExit -Command \"& {{\"{AppContext.BaseDirectory}\\Media.exe\" completion powershell | Out-String | Invoke-Expression; \"{AppContext.BaseDirectory}\\Media.exe\"}}",
                UseShellExecute = false,
            }
        };
        process.Start();
    }

    [SupportedOSPlatform("windows")]
    public override int Execute(CommandContext context)
    {
        bool shellStarted = WasItStartedByAShell();

        if (PlarformCheck.IsWindows() && !shellStarted)
        {
            StartInPowerShell();
        }
        else
        {
            PrintReleaseNotes();
        }

        return ExitCodes.Success;
    }
}
