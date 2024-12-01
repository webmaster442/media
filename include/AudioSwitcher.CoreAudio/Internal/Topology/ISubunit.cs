using System.Runtime.InteropServices;

using AudioSwitcher.CoreAudio.Internal.Interfaces;

namespace AudioSwitcher.CoreAudio.Internal.Topology;

[Guid(ComInterfaceIds.SUBUNIT_IID)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ISubunit
{

}
