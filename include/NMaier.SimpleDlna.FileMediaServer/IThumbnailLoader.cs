using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.FileMediaServer;

internal interface IThumbnailLoader
{
    DlnaMediaTypes Handling { get; }

    MemoryStream GetThumbnail(object item, ref int width, ref int height);
}