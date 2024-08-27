using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

namespace Media.Infrastructure.Dlna;

internal sealed record class SimpleFileItem : IMediaResource
{
    private readonly FileInfo _fileInfo;

    public SimpleFileItem(string file)
    {
        _fileInfo = new FileInfo(file);
        Id = Random.Shared.Next(0).ToString();
    }

    public DlnaMediaTypes MediaType => DlnaMediaTypes.Video;

    public string PN => DlnaMaps.MainPN[Type];

    public DlnaMime Type => DlnaMime.VideoAVC;

    public string Id { get; set; }

    public string Path => _fileInfo.FullName;

    public IHeaders Properties => new Headers();

    public string Title => _fileInfo.Name;

    public IMediaCoverResource Cover => new MediaCoverResource(this);

    public int CompareTo(IMediaItem? other)
    {
        return string.Compare(Title, other?.Title, StringComparison.Ordinal);
    }

    public Stream CreateContentStream()
    {
        throw new NotImplementedException();
    }

    public bool Equals(IMediaItem? other)
    {
        return CompareTo(other) == 0;
    }

    public string ToComparableTitle()
    {
        return Title;
    }
}
