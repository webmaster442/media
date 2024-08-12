using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Interop.CdRip;

internal delegate void OnReadingTrack(byte[] buffer);
internal delegate void OnTrackReadingProgress(uint bytesRead, uint totalBytes);

internal sealed class TrackReader
{
    private CdDrive _drive;

    public event OnTrackReadingProgress Progress = delegate { };

    public TrackReader(CdDrive drive)
    {
        _drive = drive;
    }

    public async Task ReadTrackAsync(Track track, OnReadingTrack onTrackRead)
    {
        await ReadTrackAsync(track, onTrackRead, CancellationToken.None);
    }

    public async Task ReadTrackAsync(Track track, OnReadingTrack onTrackRead, CancellationToken token)
    {
        await ReadTrackAsync(track.Offset, track.Sectors, onTrackRead, token);
    }

    public async Task ReadTrackAsync(int offset, int sectors, OnReadingTrack onTrackRead, CancellationToken token)
    {
        var bytes2Read = (uint)(sectors) * Constants.CB_AUDIO;
        var bytesRead = (uint)0;

        Progress(bytesRead, bytes2Read);

        for (int sector = 0; (sector < sectors); sector += Constants.NSECTORS)
        {
            if (token.IsCancellationRequested)
                return;

            var sectors2Read = ((sector + Constants.NSECTORS) < sectors) ? Constants.NSECTORS : (sectors - sector);
            var buffer = await _drive.ReadSector(offset - 150 + sector, sectors2Read);//No 2 second lead in for reading the track

            onTrackRead(buffer);
            bytesRead += (uint)(Constants.CB_AUDIO * sectors2Read);

            Progress(bytesRead, bytes2Read);
        }
    }
}
