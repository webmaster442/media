namespace NMaier.SimpleDlna.FileMediaServer;

internal sealed class Thumbnail : IThumbnail
{
    private readonly byte[] _data;

    internal Thumbnail(int width, int height, byte[] data)
    {
        Width = width;
        Height = height;
        _data = data;
    }

    public int Height { get; }

    public int Width { get; }

    public byte[] GetData()
    {
        return _data;
    }
}