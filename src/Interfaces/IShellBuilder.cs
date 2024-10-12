// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interfaces;

internal interface IShellBuilder : IBuilder<string>
{
    void WithClear();
    void WithCommandIfFileNotExists(string filePath, string command);
    void WithUtf8Enabled();
    void WithWindowTitle(string title);
}