using NMaier.SimpleDlna.Server.Metadata;

namespace NMaier.SimpleDlna.Server.Comparers;

internal class FileSizeComparer : TitleComparer
{
    public override string Description => "Sort by file size";

    public override string Name => "size";

    public override int Compare(IMediaItem? x, IMediaItem? y)
    {
        if (x is not IMetaInfo xm
         || y is not IMetaInfo ym
         || !xm.InfoSize.HasValue
         || !ym.InfoSize.HasValue)
        {
            return base.Compare(x, y);
        }
        var rv = xm.InfoSize.Value.CompareTo(ym.InfoSize.Value);
        return rv != 0 ? rv : base.Compare(x, y);
    }
}
