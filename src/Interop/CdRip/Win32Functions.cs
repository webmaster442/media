// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Media.Interop.CdRip;
internal static class Win32Functions
{
    //DesiredAccess values
    public const uint GENERIC_READ = 0x80000000;
    public const uint GENERIC_WRITE = 0x40000000;
    public const uint GENERIC_EXECUTE = 0x20000000;
    public const uint GENERIC_ALL = 0x10000000;

    //Share constants
    public const uint FILE_SHARE_READ = 0x00000001;
    public const uint FILE_SHARE_WRITE = 0x00000002;
    public const uint FILE_SHARE_DELETE = 0x00000004;

    //CreationDisposition constants
    public const uint CREATE_NEW = 1;
    public const uint CREATE_ALWAYS = 2;
    public const uint OPEN_EXISTING = 3;
    public const uint OPEN_ALWAYS = 4;
    public const uint TRUNCATE_EXISTING = 5;

    public const uint IOCTL_CDROM_READ_TOC = 0x00024000;
    public const uint IOCTL_STORAGE_CHECK_VERIFY = 0x002D4800;
    public const uint IOCTL_CDROM_RAW_READ = 0x0002403E;
    public const uint IOCTL_STORAGE_MEDIA_REMOVAL = 0x002D4804;
    public const uint IOCTL_STORAGE_EJECT_MEDIA = 0x002D4808;
    public const uint IOCTL_STORAGE_LOAD_MEDIA = 0x002D480C;

    [StructLayout(LayoutKind.Sequential)]
    public struct TRACK_DATA
    {
        public byte Reserved;
        private byte BitMapped;
        public byte Control
        {
            get => (byte)(BitMapped & 0x0F);
            set => BitMapped = (byte)((BitMapped & 0xF0) | (value & (byte)0x0F));
        }
        public byte Adr
        {
            get => (byte)((BitMapped & (byte)0xF0) >> 4);
            set => BitMapped = (byte)((BitMapped & (byte)0x0F) | (value << 4));
        }
        public byte TrackNumber;
        public byte Reserved1;

        /// <summary>
        /// Don't use array to avoid array creation
        /// </summary>
        public byte Address_0;
        public byte Address_1;
        public byte Address_2;
        public byte Address_3;
    };

    public const int MAXIMUM_NUMBER_TRACKS = 100;

    [StructLayout(LayoutKind.Sequential)]
    public class TrackDataList
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMUM_NUMBER_TRACKS * 8)]
        private readonly byte[] Data;

        public TRACK_DATA this[int Index]
        {
            get
            {
                if ((Index < 0) | (Index >= MAXIMUM_NUMBER_TRACKS))
                {
                    throw new IndexOutOfRangeException();
                }
                TRACK_DATA res;
                GCHandle handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
                try
                {
                    nint buffer = handle.AddrOfPinnedObject();
                    buffer = buffer + (Index * Marshal.SizeOf(typeof(TRACK_DATA)));
                    res = (TRACK_DATA)Marshal.PtrToStructure(buffer, typeof(TRACK_DATA))!;
                }
                finally
                {
                    handle.Free();
                }
                return res;
            }
        }

        public TrackDataList()
        {
            Data = new byte[MAXIMUM_NUMBER_TRACKS * Marshal.SizeOf(typeof(TRACK_DATA))];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class CDROM_TOC
    {
        public ushort Length;
        public byte FirstTrack = 0;
        public byte LastTrack = 0;

        public TrackDataList TrackData;

        public CDROM_TOC()
        {
            TrackData = new TrackDataList();
            Length = (ushort)Marshal.SizeOf(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class PREVENT_MEDIA_REMOVAL
    {
        public byte PreventMediaRemoval = 0;
    }

    public enum TRACK_MODE_TYPE { YellowMode2, XAForm2, CDDA }
    [StructLayout(LayoutKind.Sequential)]
    public class RAW_READ_INFO
    {
        public long DiskOffset = 0;
        public uint SectorCount = 0;
        public TRACK_MODE_TYPE TrackMode = TRACK_MODE_TYPE.CDDA;
    }

    [System.Runtime.InteropServices.DllImport("Kernel32.dll", SetLastError = true)]
    public extern static IntPtr CreateFile(string FileName,
                                           uint DesiredAccess,
                                           uint ShareMode,
                                           IntPtr lpSecurityAttributes,
                                           uint CreationDisposition,
                                           uint dwFlagsAndAttributes,
                                           IntPtr hTemplateFile);

    [System.Runtime.InteropServices.DllImport("Kernel32.dll", SetLastError = true)]
    public extern static int CloseHandle(IntPtr hObject);

    [System.Runtime.InteropServices.DllImport("Kernel32.dll", SetLastError = true)]
    public extern static int DeviceIoControl(IntPtr hDevice,
                                             uint IoControlCode,
                                             IntPtr lpInBuffer,
                                             uint InBufferSize,
                                             IntPtr lpOutBuffer,
                                             uint nOutBufferSize,
                                             ref uint lpBytesReturned,
                                             IntPtr lpOverlapped);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public extern static int DeviceIoControl(IntPtr hDevice,
                                             uint IoControlCode,
                                             IntPtr InBuffer,
                                             uint InBufferSize,
                                             [Out] CDROM_TOC OutTOC,
                                             uint OutBufferSize,
                                             ref uint BytesReturned,
                                             IntPtr Overlapped);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public extern static int DeviceIoControl(IntPtr hDevice,
                                             uint IoControlCode,
                                             [In] PREVENT_MEDIA_REMOVAL InMediaRemoval,
                                             uint InBufferSize,
                                             IntPtr OutBuffer,
                                             uint OutBufferSize,
                                             ref uint BytesReturned,
                                             IntPtr Overlapped);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public extern static int DeviceIoControl(IntPtr hDevice,
                                             uint IoControlCode,
                                             [In] RAW_READ_INFO rri,
                                             uint InBufferSize,
                                             [In, Out] byte[] OutBuffer,
                                             uint OutBufferSize,
                                             ref uint BytesReturned,
                                             IntPtr Overlapped);

}
