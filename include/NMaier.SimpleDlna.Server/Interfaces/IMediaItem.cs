namespace NMaier.SimpleDlna.Server.Interfaces;

public interface IMediaItem
: IComparable<IMediaItem>, IEquatable<IMediaItem>, ITitleComparable
{
    string Id { get; set; }

    string Path { get; }

    IHeaders Properties { get; }

    string Title { get; }
}