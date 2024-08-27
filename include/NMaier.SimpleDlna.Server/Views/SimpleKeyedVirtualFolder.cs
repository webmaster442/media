using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.Server.Views;

internal class SimpleKeyedVirtualFolder : KeyedVirtualFolder<VirtualFolder>
{
    public SimpleKeyedVirtualFolder()
    {
    }

    public SimpleKeyedVirtualFolder(IMediaFolder aParent, string aName)
      : base(aParent, aName)
    {
    }
}
