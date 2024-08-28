namespace NMaier.SimpleDlna.FileMediaServer;

public interface IThumbnail
{
    int Height { get; }

    int Width { get; }

    byte[] GetData();
}