namespace NMaier.SimpleDlna.Server.Utilities;

public sealed class ConcatenatedStream : Stream
{
    private readonly Queue<Stream> _streams = new Queue<Stream>();

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length
    {
        get { throw new NotSupportedException(); }
    }

    public override long Position
    {
        get { throw new NotSupportedException(); }
        set { throw new NotSupportedException(); }
    }

    public void AddStream(Stream stream)
    {
        _streams.Enqueue(stream);
    }

    public override void Close()
    {
        foreach (var stream in _streams)
        {
            stream.Close();
            stream.Dispose();
        }
        _streams.Clear();
        base.Close();
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_streams.Count == 0)
        {
            return 0;
        }

        var read = _streams.Peek().Read(buffer, offset, count);
        if (read < count)
        {
            var sndRead = _streams.Peek().Read(buffer, offset + read, count - read);
            if (sndRead <= 0)
            {
                _streams.Dequeue().Dispose();
                return read + Read(buffer, offset + read, count - read);
            }
            read += sndRead;
        }
        return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
}