using System.Runtime.InteropServices;

namespace AudioSwitcher.CoreAudio.Internal.Interfaces;

[Guid(ComInterfaceIds.AUDIO_ENDPOINT_VOLUME_CALLBACK_IID)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioEndpointVolumeCallback
{
    [PreserveSig]
    int OnNotify([In] nint notificationData);
}