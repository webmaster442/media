using System;
using System.Runtime.InteropServices;

using AudioSwitcher.CoreAudio.Internal.Interfaces;

namespace AudioSwitcher.CoreAudio.Internal.Topology;

[Guid(ComInterfaceIds.CONTROL_INTERFACE_IID)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IControlInterface
{
    [PreserveSig]
    int GetName([Out][MarshalAs(UnmanagedType.LPWStr)] out string name);

    [PreserveSig]
    int GetInterfaceId([Out] out Guid interfaceId); //Note this was GetIID, hoping the vtable is enough
}
