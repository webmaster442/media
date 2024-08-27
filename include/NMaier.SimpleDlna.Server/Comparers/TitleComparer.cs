using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Comparers;

internal class TitleComparer : BaseComparer
{
    private static readonly StringComparer Coparer =
      new NaturalStringComparer(false);

    public override string Description => "Sort alphabetically";

    public override string Name => "title";

    public override int Compare(IMediaItem? x, IMediaItem? y)
    {
        if (x == null && y == null)
        {
            return 0;
        }
        if (x == null)
        {
            return 1;
        }
        if (y == null)
        {
            return -1;
        }
        return Coparer.Compare(x.ToComparableTitle(), y.ToComparableTitle());
    }
}