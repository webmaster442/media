// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interop;

internal sealed class ShellHelper
{
    private readonly string _path;
    public ShellHelper()
    {
        _path = Path.Combine(AppContext.BaseDirectory, "Media.ShellHelper.exe");
    }

    public string SetProgress(int total, int current)
        => $"{_path} progress {total} {current}";

    public string HideProgress()
        => $"{_path} hideprogress";
}
