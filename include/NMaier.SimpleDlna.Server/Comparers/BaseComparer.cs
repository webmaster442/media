using NMaier.SimpleDlna.Server.Interfaces;

namespace NMaier.SimpleDlna.Server.Comparers;

public abstract class BaseComparer : IItemComparer
{
    public abstract string Description { get; }

    public abstract string Name { get; }

    public abstract int Compare(IMediaItem? x, IMediaItem? y);

    public override string ToString()
    {
        return $"{Name} - {Description}";
    }
}