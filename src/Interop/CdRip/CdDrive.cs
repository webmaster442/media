using System.Runtime.InteropServices;

namespace Media.Interop.CdRip;

//https://github.com/LodewijkSioen/CdRipper.net/blob/master/src/CdRipper.Console/Program.cs
internal sealed class CdDrive : IDisposable
{
    private IntPtr _driveHandle;

    private string _driveName;

    public static CdDrive Create(DriveInfo drive)
    {
        return Create(drive.Name[..1]);
    }

    public static CdDrive Create(string driveName)
    {
        return new CdDrive(driveName);
    }

    private CdDrive(string driveName)
    {
        if (Win32Functions.GetDriveType(driveName + ":\\") != Win32Functions.DriveTypes.DRIVE_CDROM)
        {
            throw new InvalidOperationException("Drive '" + driveName + "' is not an optical disc drive.");
        }

        _driveName = driveName;
        _driveHandle = CreateDriveHandle().Result;
    }

    public async Task<bool> IsCdInDrive()
    {
        return await Task.Run(() =>
        {
            uint dummy = 0;
            var check = Win32Functions.DeviceIoControl(_driveHandle, Win32Functions.IOCTL_STORAGE_CHECK_VERIFY,
                IntPtr.Zero, 0, IntPtr.Zero, 0, ref dummy, IntPtr.Zero);
            return check != 0;
        });
    }

    public async Task<TableOfContents?> ReadTableOfContents()
    {
        if (!await IsCdInDrive())
        {
            return null;
        }

        return await Task.Run(() =>
        {
            var toc = new Win32Functions.CDROM_TOC();
            uint bytesRead = 0;
            var succes =
                Win32Functions.DeviceIoControl(_driveHandle, Win32Functions.IOCTL_CDROM_READ_TOC, IntPtr.Zero, 0,
                    toc, (uint)Marshal.SizeOf(toc), ref bytesRead, IntPtr.Zero) != 0;
            if (!succes)
            {
                throw new Exception("Reading the TOC failed.");
            }
            return TableOfContents.Create(toc);
        });
    }

    public async Task<byte[]> ReadSector(int startSector, int numberOfSectors)
    {
        return await Task.Run(() =>
        {
            var rri = new Win32Functions.RAW_READ_INFO
            {
                TrackMode = Win32Functions.TRACK_MODE_TYPE.CDDA,
                SectorCount = (uint)numberOfSectors,
                DiskOffset = startSector * Constants.CB_CDROMSECTOR
            };

            uint bytesRead = 0;
            var buffer = new byte[Constants.CB_AUDIO * numberOfSectors];
            var success = Win32Functions.DeviceIoControl(_driveHandle, Win32Functions.IOCTL_CDROM_RAW_READ, rri,
                (uint)Marshal.SizeOf(rri), buffer, (uint)buffer.Length, ref bytesRead,
                IntPtr.Zero);

            if (success != 0)
            {
                return buffer;
            }
            throw new Exception("Failed to read from disk");
        });
    }

    private static bool IsValidHandle(IntPtr handle) 
        => ((int)handle != -1) && ((int)handle != 0);

    public async Task<bool> Lock()
    {
        return await Task.Run(() =>
        {
            uint dummy = 0;
            var pmr = new Win32Functions.PREVENT_MEDIA_REMOVAL { PreventMediaRemoval = 1 };
            return
                Win32Functions.DeviceIoControl(_driveHandle, Win32Functions.IOCTL_STORAGE_MEDIA_REMOVAL, pmr,
                    (uint)Marshal.SizeOf(pmr), IntPtr.Zero, 0, ref dummy, IntPtr.Zero) != 0;
        });
    }

    public async Task<bool> UnLock()
    {
        return await Task.Run(() =>
        {
            uint dummy = 0;
            var pmr = new Win32Functions.PREVENT_MEDIA_REMOVAL { PreventMediaRemoval = 0 };
            return Win32Functions.DeviceIoControl(_driveHandle, Win32Functions.IOCTL_STORAGE_MEDIA_REMOVAL, pmr, (uint)Marshal.SizeOf(pmr), IntPtr.Zero, 0, ref dummy, IntPtr.Zero) == 0;
        });
    }

    public async Task<bool> Eject()
    {
        return await Task.Run(() =>
        {
            uint Dummy = 0;
            return Win32Functions.DeviceIoControl(_driveHandle, Win32Functions.IOCTL_STORAGE_EJECT_MEDIA, IntPtr.Zero, 0, IntPtr.Zero, 0, ref Dummy, IntPtr.Zero) != 0;
        });
    }

    private async Task<IntPtr> CreateDriveHandle()
    {
        return await Task.Run(() =>
        {
            var handle = Win32Functions.CreateFile("\\\\.\\" + _driveName + ':', Win32Functions.GENERIC_READ,
                Win32Functions.FILE_SHARE_READ, IntPtr.Zero, Win32Functions.OPEN_EXISTING, 0, IntPtr.Zero);
            if (IsValidHandle(handle))
            {
                return handle;
            }
            throw new InvalidOperationException("Drive '" + _driveName + "' is currently opened.");
        });
    }

    public void Dispose()
    {
        UnLock().Wait();
        Win32Functions.CloseHandle(_driveHandle);
        _driveHandle = IntPtr.Zero;
        _driveName = string.Empty;
        GC.SuppressFinalize(this);
    }

    ~CdDrive()
    {
        Dispose();
    }
}