using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.Server.Views;

internal class SimpleKeyedVirtualFolder : KeyedVirtualFolder<VirtualFolder>, ICreatable<SimpleKeyedVirtualFolder>
{
    public SimpleKeyedVirtualFolder(IMediaFolder? aParent, string aName)
      : base(aParent, aName)
    {
    }

    static SimpleKeyedVirtualFolder ICreatable<SimpleKeyedVirtualFolder>.Create(string key, IMediaFolder? parent)
    {
        return new SimpleKeyedVirtualFolder(parent, key);
    }
}
