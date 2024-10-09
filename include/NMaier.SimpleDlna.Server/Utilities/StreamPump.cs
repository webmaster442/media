using Microsoft.Extensions.Logging;

namespace NMaier.SimpleDlna.Server.Utilities;

public sealed class StreamPump : IDisposable
{
    private readonly byte[] _buffer;
    private bool _disposed;

    private readonly SemaphoreSlim _sem;
    private readonly ILogger _log;

    public StreamPump(Stream inputStream, Stream outputStream, int bufferSize, ILogger log)
    {
        _sem = new SemaphoreSlim(0, 1);
        _disposed = false;
        _buffer = new byte[bufferSize];
        Input = inputStream;
        Output = outputStream;
        _log = log;
    }

    public Stream Input { get; }

    public Stream Output { get; }

    public void Dispose()
    {
        if (!_disposed)
        {
            _sem.Dispose();
            _disposed = true;
        }
    }

    private void Finish(StreamPumpResult result, StreamPumpCallback? callback)
    {
        callback?.Invoke(this, result);
        try
        {
            if (!_disposed)
            {
                _sem.Release();
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Exception: {ex}", ex.Message);
        }
    }

    public void Pump(StreamPumpCallback? callback)
    {
        try
        {
            Input.BeginRead(_buffer, 0, _buffer.Length, readResult =>
            {
                try
                {
                    var read = Input.EndRead(readResult);
                    if (read <= 0)
                    {
                        Finish(StreamPumpResult.Delivered, callback);
                        return;
                    }

                    try
                    {
                        Output.BeginWrite(_buffer, 0, read, writeResult =>
                        {
                            try
                            {
                                Output.EndWrite(writeResult);
                                Pump(callback);
                            }
                            catch (Exception)
                            {
                                Finish(StreamPumpResult.Aborted, callback);
                            }
                        }, null);
                    }
                    catch (Exception)
                    {
                        Finish(StreamPumpResult.Aborted, callback);
                    }
                }
                catch (Exception)
                {
                    Finish(StreamPumpResult.Aborted, callback);
                }
            }, null);
        }
        catch (Exception)
        {
            Finish(StreamPumpResult.Aborted, callback);
        }
    }

    public bool Wait(int timeout)
    {
        return _sem.Wait(timeout);
    }
}