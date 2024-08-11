namespace Media.Interop.CdRip;

internal sealed class TableOfContents
{
    private const int Leadout = ((60 + 90 + 2) * 75); //https://github.com/metabrainz/libdiscid/blob/master/src/toc.c This has some wierd stuff in it..
    public IList<Track> Tracks { get; private set; }

    public TableOfContents(IList<Track> tracks)
    {
        Tracks = tracks;
    }

    private static IEnumerable<Track> GetTracks(Win32Functions.CDROM_TOC toc)
    {
        for (var i = toc.FirstTrack - 1; i < toc.LastTrack; i++)
        {
            if (toc.TrackData[i].Control == 0)
            {
                var trackNumber = toc.TrackData[i].TrackNumber;
                var offset = GetStartSector(toc.TrackData[i]);

                var nextTrack = toc.TrackData[i + 1];
                var sectors = GetStartSector(nextTrack) - offset;

                yield return new Track(trackNumber, offset, sectors);
            }
        }
    }

    private static int GetStartSector(Win32Functions.TRACK_DATA data)
    {
        return (data.Address_1 * 60 * 75 + data.Address_2 * 75 + data.Address_3) - (data.Control == 0 ? 0 : Leadout);
    }

    internal static TableOfContents Create(Win32Functions.CDROM_TOC toc)
    {
        var tracks = GetTracks(toc).ToList();
        return new TableOfContents(tracks);
    }
}