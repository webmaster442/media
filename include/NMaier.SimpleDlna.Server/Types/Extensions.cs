namespace NMaier.SimpleDlna.Server.Types;

public static class Extensions
{
    public static IEnumerable<string> GetExtensions(this DlnaMediaTypes types)
    {
        return (from i in DlnaMaps.Media2Ext
                where types.HasFlag(i.Key)
                select i.Value).SelectMany(i => i);
    }
}