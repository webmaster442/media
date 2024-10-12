using System.Diagnostics;

namespace Media.Interop;

internal sealed class Powershell
{
    private readonly string _powershellExecutable;

    private static string? FindPowershellCore()
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

    public Powershell()
    {
        _powershellExecutable = FindPowershellCore() ?? "powershell.exe";
    }

    public void RunCommands(IEnumerable<string> commands)
    {
        string cmd = string.Join(";", commands);

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _powershellExecutable,
                Arguments = $"-NoExit -Command \"& {{{cmd}}}",
                UseShellExecute = false,
            }
        };
        process.Start();
    }

    public void RunScript(string scriptFile, bool noExit = false)
    {
        if (!File.Exists(scriptFile))
        {
            throw new FileNotFoundException("Script file not found", scriptFile);
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _powershellExecutable,
                Arguments = noExit 
                            ? $"-executionpolicy bypass -noexit -File \"{scriptFile}\""
                            : $"-executionpolicy bypass -File \"{scriptFile}\"",
                UseShellExecute = false,
            }
        };
        process.Start();
    }
}