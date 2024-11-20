using System;

using AudioSwitcher.CoreAudio.Internal.Interfaces;

namespace AudioSwitcher.CoreAudio.Internal;

internal static class ComObjectFactory
{
    public static IMultimediaDeviceEnumerator GetDeviceEnumerator()
    {
        return Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(ComInterfaceIds.DEVICE_ENUMERATOR_CID))) as IMultimediaDeviceEnumerator;
    }

    public static object GetPolicyConfig()
    {
        return Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid(ComInterfaceIds.POLICY_CONFIG_CID)));
    }

}
