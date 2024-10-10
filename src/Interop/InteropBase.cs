// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Media.Infrastructure;

namespace Media.Interop;

public abstract class InteropBase
{
    private readonly string _programName;

    protected InteropBase(string binaryName)
    {
        _programName = binaryName;
    }

    protected abstract string? GetExternalPath();

    public virtual bool TryGetInstalledPath([NotNullWhen(true)] out string? toolPath)
    {
        var externalPath = GetExternalPath();
        if (externalPath != null
            && File.Exists(externalPath))
        {
            toolPath = externalPath;
            return true;
        }

        var appBundledPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _programName);
        if (File.Exists(appBundledPath))
        {
            toolPath = appBundledPath;
            return true;
        }

        toolPath = null;
        return false;
    }

    public Process CreateProcess(string commandLine, bool redirectStdIn, bool redirectStdOut, bool redirectStderr)
    {
        if (!TryGetInstalledPath(out string? toolPath))
        {
            throw new InvalidOperationException($"{_programName} not found.");
        }

        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = toolPath,
                Arguments = commandLine,
                UseShellExecute = false,
                RedirectStandardInput = redirectStdIn,
                RedirectStandardOutput = redirectStdOut,
                RedirectStandardError = redirectStderr,
            }
        };
    }

    public void Start(string commandLine)
    {
        using var process = CreateProcess(commandLine,
                                          redirectStdIn: false,
                                          redirectStdOut: false,
                                          redirectStderr: false);

        process.Start();
    }
}
