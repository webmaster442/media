using System.Runtime.InteropServices;

namespace AudioSwitcher.CoreAudio.Internal.Interfaces;

[Guid(ComInterfaceIds.AUDIO_SESSION_NOTIFICATION_IID)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioSessionNotification
{
    [PreserveSig]
    int OnSessionCreated([In] IAudioSessionControl sessionControl);
}