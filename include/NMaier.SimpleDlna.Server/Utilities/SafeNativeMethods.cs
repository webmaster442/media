using System.Runtime.InteropServices;

namespace NMaier.SimpleDlna.Server.Utilities;

internal static class SafeNativeMethods
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    internal static extern int StrCmpLogicalW(string psz1, string psz2);

    [DllImport("iphlpapi.dll")]
    public static extern uint SendARP(uint destIP, uint srcIP, [Out] byte[] pMacAddr, ref uint phyAddrLen);
}