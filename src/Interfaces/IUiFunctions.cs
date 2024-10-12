namespace Media.Interfaces;

internal interface IUiFunctions
{
    bool QuestionMessage(string message, string title);
    void WarningMessage(string message, string title);
    void InfoMessage(string message, string title);
    void ErrorMessage(string message, string title);
    string? SelectFolderDialog(string? startFolder);
    void Exit(int exitCode);
}
