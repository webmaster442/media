namespace Media.Interop;

internal interface IShellBuilder : IBuilder<string>
{
    void WithClear();
    void WithCommandIfFileNotExists(string filePath, string command);
    void WithUtf8Enabled();
    void WithWindowTitle(string title);
}