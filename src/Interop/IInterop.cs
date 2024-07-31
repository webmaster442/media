namespace Media.Interop;

internal interface IInterop
{
    public abstract static bool TryGetInstalledPath(out string toolPath);
    public abstract static void EnsureIsInstalled();
    public abstract static void Start(string commandLine);
}
