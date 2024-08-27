using NMaier.SimpleDlna.Server.Metadata;

namespace NMaier.SimpleDlna.Server.Comparers;

internal class DateComparer : TitleComparer
{
    public override string Description => "Sort by file date";

    public override string Name => "date";

    public override int Compare(IMediaItem? x, IMediaItem? y)
    {
        if (x is IMetaInfo xm
         && y is IMetaInfo ym)
        {
            var rv = xm.InfoDate.CompareTo(ym.InfoDate);
            if (rv != 0)
            {
                return rv;
            }
        }
        return base.Compare(x, y);
    }
}
