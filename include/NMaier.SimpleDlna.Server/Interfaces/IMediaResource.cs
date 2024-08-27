using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.Server.Interfaces;

public interface IMediaResource : IMediaItem, IMediaCover
{
    DlnaMediaTypes MediaType { get; }

    string PN { get; }

    DlnaMime Type { get; }

    Stream CreateContentStream();
}