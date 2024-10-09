using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.FileMediaServer.Properties;
using NMaier.SimpleDlna.Server.Types;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.FileMediaServer;

public sealed class ThumbnailMaker
{
    private byte[] GetThumbnailInternal(DlnaMediaTypes type)
    {
        return type switch
        {
            DlnaMediaTypes.Video => Resources.movie,
            DlnaMediaTypes.Audio => Resources.music,
            DlnaMediaTypes.Image => Resources.image,
            DlnaMediaTypes.All => Resources.folder,
            _ => throw new InvalidOperationException("Invalid type"),
        };
    }

    public IThumbnail GetThumbnail(FileSystemInfo file, int width, int height)
    {
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }
        var ext = file.Extension.ToUpperInvariant().Substring(1);
        var mediaType = DlnaMaps.Ext2Media[ext];

        var key = file.FullName;

        return new Thumbnail(192, 192, GetThumbnailInternal(mediaType));
    }

    public IThumbnail GetThumbnail(string key, DlnaMediaTypes type, Stream stream, int width, int height)
    {
        return new Thumbnail(192, 192, GetThumbnailInternal(type));
    }
}