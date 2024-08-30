using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.Server.Views;

internal class DoubleKeyedVirtualFolder
: KeyedVirtualFolder<SimpleKeyedVirtualFolder>, ICreatable<DoubleKeyedVirtualFolder>
{
    public DoubleKeyedVirtualFolder(IMediaFolder? aParent, string aName)
        : base(aParent, aName)
    {
    }

    static DoubleKeyedVirtualFolder ICreatable<DoubleKeyedVirtualFolder>.Create(string key, IMediaFolder? parent)
    {
        return new DoubleKeyedVirtualFolder(parent, key);
    }
}
