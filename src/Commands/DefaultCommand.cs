// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;

using Media.Dto.Cli;
using Media.Infrastructure;
using Media.Interop;

using Spectre.Console;

namespace Media.Commands;

internal class DefaultCommand : Command
{
    private static void PrintReleaseNotes(string tree)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Media.Notes.txt");
        if (stream == null)
            throw new InvalidOperationException("Can't find notes");

        using var reader = new StreamReader(stream);

        var content = reader.ReadToEnd();

        AnsiConsole.WriteLine(content);

        AnsiConsole.WriteLine("Available commands: \r\n");
        AnsiConsole.MarkupLine($"[yellow]{tree}[/]");

        AnsiConsole.WriteLine("\r\nExamples: ");

        var exampleGenerator = new ExampleGenerator();
        foreach (var example in exampleGenerator.GenerateExamples())
        {
            AnsiConsole.MarkupLine(example);
        }
    }

    private static bool WasItStartedByAShell()
    {
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

    private static string BuildTree(Model mdl)
    {
        static char GetTreeChar(int length, int i)
        {
            if (length == 1 || i == (length - 1))
                return '└';

            return '├';
        }

        StringBuilder sb = new(256 + 512);

        var sorted = mdl.Commands.OrderBy(c => c.Name).ToArray();

        sb.AppendLine("Media");

        for (int i = 0; i < sorted.Length; i++)
        {
            if (sorted[i].IsDefault)
                continue;

            sb.Append("  ");
            sb.Append(GetTreeChar(sorted.Length, i));
            sb.Append(' ');
            sb.AppendLine(sorted[i].Name);

            var subCommands = sorted[i].Commands;

            if (subCommands != null)
            {
                for (int j = 0; j < subCommands.Length; j++)
                {
                    sb.Append("  ");
                    sb.Append("│ ");
                    sb.Append(GetTreeChar(subCommands.Length, j));
                    sb.Append(' ');
                    sb.AppendLine(subCommands[j].Name);
                }
            }
        }
        return sb.ToString();
    }

    [SupportedOSPlatform("windows")]
    public override int Execute(CommandContext context)
    {
        var mdl = CliModelProvider.GetModel();

        string tree = BuildTree(mdl);

        bool shellStarted = WasItStartedByAShell();
        Powershell powershell = new();
        if (!shellStarted && !Debugger.IsAttached)
        {
            powershell.RunCommands(new string[]
            {
                $"\"{AppContext.BaseDirectory}\\Media.exe\" completion powershell | Out-String | Invoke-Expression",
                $"\"{AppContext.BaseDirectory}\\Media.exe\""
            });
        }
        else
        {
            PrintReleaseNotes(tree);
        }

        return ExitCodes.Success;
    }
}