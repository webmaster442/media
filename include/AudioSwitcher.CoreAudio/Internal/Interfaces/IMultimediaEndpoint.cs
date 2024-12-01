﻿using System.Runtime.InteropServices;

namespace AudioSwitcher.CoreAudio.Internal.Interfaces;

[Guid(ComInterfaceIds.IMM_ENDPOINT_IID)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMultimediaEndpoint
{
    [PreserveSig]
    int GetDataFlow([Out][MarshalAs(UnmanagedType.I4)] out EDataFlow dataFlow);
}