// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interop.CdRip;

internal sealed class TrackReader
{
    private readonly CdDrive _drive;

    public TrackReader(CdDrive drive)
    {
        _drive = drive;
    }

    public async Task ReadTrackAsync(Track track, Action<byte[]> onTrackRead, Action<long, long> progress, CancellationToken token)
    {
        var bytes2Read = (uint)(track.Sectors) * Constants.CB_AUDIO;
        var bytesRead = (uint)0;


        progress(bytesRead, bytes2Read);

        for (int sector = 0; (sector < track.Sectors); sector += Constants.NSECTORS)
        {
            if (token.IsCancellationRequested)
                return;

            var sectors2Read = ((sector + Constants.NSECTORS) < track.Sectors) ? Constants.NSECTORS : (track.Sectors - sector);
            var buffer = await _drive.ReadSector(track.Offset - 150 + sector, sectors2Read);//No 2 second lead in for reading the track

            onTrackRead(buffer);
            bytesRead += (uint)(Constants.CB_AUDIO * sectors2Read);

            progress(bytesRead, bytes2Read);
        }
    }
}
