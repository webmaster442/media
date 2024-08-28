using File = TagLib.File;

namespace NMaier.SimpleDlna.FileMediaServer.Files;

internal sealed class TagLibFileAbstraction : File.IFileAbstraction
{
    private readonly FileInfo _info;

    public TagLibFileAbstraction(FileInfo info)
    {
        this._info = info;
    }

    public string Name => _info.FullName;

    public Stream ReadStream => _info.Open(
      FileMode.Open,
      FileAccess.Read,
      FileShare.ReadWrite
      );

    public Stream WriteStream
    {
        get { throw new NotImplementedException(); }
    }

    public void CloseStream(Stream stream)
    {
        if (stream == null)
        {
            return;
        }
        stream.Close();
        stream.Dispose();
    }
}