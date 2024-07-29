namespace Media.Interop;

internal class PowershellBuilder : IBuilder<string>
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

    public PowershellBuilder WithClear()
    {
        _scriptBuilder.AppendLine("Clear-Host");
        return this;
    }

    public PowershellBuilder WithCommand(string command)
    {
        _scriptBuilder.AppendLine(command);
        return this;
    }

    public PowershellBuilder WithCommandIfFileExists(string filePath, string command)
    {
        _scriptBuilder.AppendLine($"if (Test-Path '{filePath}') {{");
        _scriptBuilder.AppendLine($"    {command}");
        _scriptBuilder.AppendLine("}");
        return this;
    }

    public PowershellBuilder WithCommandIfFileNotExists(string filePath, string command)
    {
        _scriptBuilder.AppendLine($"if (-not (Test-Path '{filePath}')) {{");
        _scriptBuilder.AppendLine($"    {command}");
        _scriptBuilder.AppendLine("}");
        return this;
    }

    public PowershellBuilder WithDeleteScript()
    {
        _scriptBuilder.AppendLine("$scriptPath = $MyInvocation.MyCommand.Path");
        _scriptBuilder.AppendLine("Remove-Item -LiteralPath $scriptPath -Force");
        return this;
    }

    public PowershellBuilder WithUtf8Enabled()
    {
        _scriptBuilder.AppendLine("[Console]::OutputEncoding = [System.Text.Encoding]::UTF8");
        return this;
    }
    public PowershellBuilder WithWindowTitle(string title)
    {
        _scriptBuilder.AppendLine($"$Host.UI.RawUI.WindowTitle = \"{title}\"");
        return this;
    }
    public PowershellBuilder WithWriteToConsole(string message)
    {
        _scriptBuilder.AppendLine($"Write-Output \"{message}\"");
        return this;
    }
}