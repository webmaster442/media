using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.Server.Views;

internal class KeyedVirtualFolder<T> : VirtualFolder
where T : VirtualFolder, ICreatable<T>
{
    private readonly Dictionary<string, T> keys = new Dictionary<string, T>(StringComparer.CurrentCultureIgnoreCase);

    protected KeyedVirtualFolder(IMediaFolder? aParent, string aName)
      : base(aParent, aName)
    {
    }

    public T GetFolder(string key)
    {
        if (!keys.TryGetValue(key, out T? rv))
        {
            rv = T.Create(key, this);
            Folders.Add(rv);
            keys.Add(key, rv);
        }
        return rv;
    }
}
