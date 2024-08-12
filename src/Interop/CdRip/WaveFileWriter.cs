using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static Media.Interop.CdRip.WaveFileWriter;

namespace Media.Interop.CdRip;
internal sealed class WaveFileWriter : IDisposable
{
    private readonly Stream _target;
    private readonly bool _leaveOpen;

    public WaveFileWriter(Stream target, bool leaveOpen)
    {
        _target = target;
        _leaveOpen = leaveOpen;
    }

    public void Dispose()
    {
        if (!_leaveOpen)
        {
            _target.Dispose();
        }
        GC.SuppressFinalize(this);
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private class WaveFormat
    {
        public short wFormatTag;
        public short nChannels;
        public int nSamplesPerSec;
        public int nAvgBytesPerSec;
        public short nBlockAlign;
        public short wBitsPerSample;
        public short cbSize;

        public WaveFormat(int rate, int bits, int channels)
        {
            wFormatTag = (short)1;
            nChannels = (short)channels;
            nSamplesPerSec = rate;
            wBitsPerSample = (short)bits;
            cbSize = 0;

            nBlockAlign = (short)(channels * (bits / 8));
            nAvgBytesPerSec = nSamplesPerSec * nBlockAlign;
        }
    }

    private static byte[] Int2ByteArr(uint val)
    {
        byte[] res = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            res[i] = (byte)(val >> (i * 8));
        }
        return res;
    }

    private static byte[] Int2ByteArr(short val)
    {
        byte[] res = new byte[2];
        for (int i = 0; i < 2; i++)
        {
            res[i] = (byte)(val >> (i * 8));
        }
        return res;
    }

    private const uint WaveHeaderSize = 38;
    private const uint WaveFormatSize = 18;

    public void WriteHeader(int sampleRate, int sampleBits, int channels, uint audioDataSize)
    {
        var formatData = new WaveFormat(sampleRate, sampleBits, channels);

        _target.Write(new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' });
        _target.Write(Int2ByteArr(audioDataSize + WaveHeaderSize));
        _target.Write(new byte[] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' });
        _target.Write(new byte[] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' });
        _target.Write(Int2ByteArr(WaveFormatSize));
        _target.Write(Int2ByteArr(formatData.wFormatTag));
        _target.Write(Int2ByteArr(formatData.nChannels));
        _target.Write(Int2ByteArr((uint)formatData.nSamplesPerSec));
        _target.Write(Int2ByteArr((uint)formatData.nAvgBytesPerSec));
        _target.Write(Int2ByteArr(formatData.nBlockAlign));
        _target.Write(Int2ByteArr(formatData.wBitsPerSample));
        _target.Write(Int2ByteArr(formatData.cbSize));
        _target.Write(new byte[] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' });
        _target.Write(Int2ByteArr(audioDataSize));
    }

    public void WriteData(byte[] buffer)
    {
        _target.Write(buffer);
    }
}
