﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interfaces;

internal interface IUiFunctions : IProgress<double>
{
    bool QuestionMessage(string message, string title);
    void WarningMessage(string message, string title);
    void InfoMessage(string message, string title);
    void ErrorMessage(string message, string title);
    string? SelectFolderDialog(string? startFolder);
    void Exit(int exitCode);
    void SetProgressState(ProgressState state);
    void BlockUi();
    void UnblockUi();
    string? OpenFileDialog(string filterString);
    string? SaveFileDialog(string filterString);
}
