// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interop;

internal class PowershellBuilder : IShellBuilder
{
    private readonly StringBuilder _scriptBuilder;

    public PowershellBuilder()
    {
        _scriptBuilder = new StringBuilder(4096);
    }

    public string Build()
    {
        return _scriptBuilder.ToString();
    }

    public void WithClear()
    {
        _scriptBuilder.AppendLine("Clear-Host");
    }

    public void WithCommandIfFileNotExists(string filePath, string command)
    {
        _scriptBuilder.AppendLine($"if (-not (Test-Path '{filePath}')) {{");
        _scriptBuilder.AppendLine($"    {command}");
        _scriptBuilder.AppendLine("}");
    }

    public void WithUtf8Enabled()
    {
        _scriptBuilder.AppendLine("[Console]::OutputEncoding = [System.Text.Encoding]::UTF8");
    }

    public void WithWindowTitle(string title)
    {
        _scriptBuilder.AppendLine($"$Host.UI.RawUI.WindowTitle = \"{title}\"");
    }
}