using System.Runtime.InteropServices;

namespace AudioSwitcher.CoreAudio.Internal;

internal static class NativeMethods
{
    [DllImport("ole32.dll")]
    public static extern int PropVariantClear(ref PropVariant pvar);
}