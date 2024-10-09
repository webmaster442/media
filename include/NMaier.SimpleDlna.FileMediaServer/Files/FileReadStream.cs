using Microsoft.Extensions.Logging;

namespace NMaier.SimpleDlna.FileMediaServer.Files;

internal sealed class FileReadStream : FileStream
{
    private readonly FileInfo info;
    private readonly ILogger _logger;
    private bool killed;

    public FileReadStream(FileInfo info, ILogger logger)
      : base(info.FullName, FileMode.Open,
             FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
             1,
             FileOptions.Asynchronous | FileOptions.SequentialScan)
    {
        this.info = info;
        _logger = logger;
        _logger.LogDebug("Opened file {filename}", this.info.FullName);
    }

    public void Kill()
    {
        _logger.LogDebug("Killed file {filename}", info.FullName);
        killed = true;
        Close();
        Dispose();
    }

    public override void Close()
    {
        if (!killed)
        {
            FileStreamCache.Recycle(this);
            return;
        }
        base.Close();
        _logger.LogDebug("Closed file {filename}", info.FullName);
    }

    protected override void Dispose(bool disposing)
    {
        if (!killed)
        {
            return;
        }
        base.Dispose(disposing);
    }
}