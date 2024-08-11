namespace Media.Interop.CdRip;

internal sealed class Track
{
    public Track(int trackNumber, int offset, int sectors)
    {
        TrackNumber = trackNumber;
        Offset = offset;
        Sectors = sectors;
    }

    public int TrackNumber { get; private set; }
    public int Offset { get; private set; }
    public int Sectors { get; private set; }

    public TimeSpan Length
    {
        get { return TimeSpan.FromSeconds(Math.Round(Sectors / 75d)); }
    }
}
