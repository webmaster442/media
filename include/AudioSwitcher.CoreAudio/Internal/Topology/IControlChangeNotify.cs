using System.Runtime.InteropServices;

using AudioSwitcher.CoreAudio.Internal.Interfaces;

namespace AudioSwitcher.CoreAudio.Internal.Topology;

[Guid(ComInterfaceIds.CONTROL_CHANGE_NOTIFY_IID)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IControlChangeNotify
{
    [PreserveSig]
    int OnNotify([In][MarshalAs(UnmanagedType.U4)] uint processId, [In, Optional][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
}
