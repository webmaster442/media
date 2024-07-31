namespace Media.Interop;

internal sealed class CmdBuilder : IShellBuilder
{
    private readonly StringBuilder _scriptBuilder;

    public CmdBuilder()
    {
        _scriptBuilder = new StringBuilder(4096);
    }

    public string Build()
    {
        return _scriptBuilder.ToString();
    }

    public void WithClear()
    {
        _scriptBuilder.AppendLine("cls");
    }

    public void WithCommandIfFileNotExists(string filePath, string command)
    {
        _scriptBuilder.AppendLine($"IF NOT EXIST \"{filePath}\" (");
        _scriptBuilder.AppendLine($"    {command}");
        _scriptBuilder.AppendLine(")");
    }

    public void WithUtf8Enabled()
    {
        _scriptBuilder.AppendLine("chcp 65001");
    }

    public void WithWindowTitle(string title)
    {
        _scriptBuilder.AppendLine($"TITLE {title}");
    }

}