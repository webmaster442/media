// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interop;

internal interface IInterop
{
    public abstract static bool TryGetInstalledPath(out string toolPath);
    public abstract static void EnsureIsInstalled();
    public abstract static int Start(string commandLine);
}
