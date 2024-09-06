using System.Runtime.InteropServices;

namespace Media.Infrastructure;

internal static class PlarformCheck
{
    public static void EnsureThatPlatformIs(params OSPlatform[] platformIDs)
    {
        bool checkPassed = false;
        foreach (var plarformId in platformIDs)
        {
            if (RuntimeInformation.IsOSPlatform(plarformId))
            {
                checkPassed = true;
                break;
            }
        }

        if (!checkPassed)
        {
            Terminal.RedText("This command is not supported on this platform.");
            Environment.Exit(ExitCodes.PlatformNotSupported);
        }
    }

    public static bool IsWindows()
        => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}
