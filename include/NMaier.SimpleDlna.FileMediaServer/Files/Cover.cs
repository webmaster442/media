using System.Runtime.Serialization;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Interfaces.Metadata;
using NMaier.SimpleDlna.Server.Types;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.FileMediaServer.Files;

[Serializable]
internal sealed class Cover
: Logging, IMediaCoverResource, IMetaInfo, ISerializable
{
    private static readonly ThumbnailMaker thumber =
      new ThumbnailMaker();

    private readonly FileInfo _file;
    private byte[]? _bytes;

    private int _height = 216;

    private bool _warned;

    private int _width = 384;

    internal Cover(FileInfo aFile, Stream aStream, ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        _file = aFile;
        var thumb = thumber.GetThumbnail(aFile.FullName,
                                         DlnaMediaTypes.Image,
                                         aStream,
                                         _width,
                                         _height);
        _bytes = thumb.GetData();
        _height = thumb.Height;
        _width = thumb.Width;
    }

    public Cover(FileInfo aFile, ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        _file = aFile;
    }

    private byte[] Bytes
    {
        get
        {
            var rv = ForceLoad();
            if (rv == null || rv.Length == 0)
            {
                throw new NotSupportedException();
            }
            return rv;
        }
    }

    IMediaCoverResource IMediaCover.Cover => this;

    public string Id
    {
        get { throw new NotSupportedException(); }
        set { throw new NotSupportedException(); }
    }

    public DlnaMediaTypes MediaType => DlnaMediaTypes.Image;

    public int? MetaHeight => _height;

    public int? MetaWidth => _width;

    public string Path
    {
        get { throw new NotSupportedException(); }
    }

    public string PN => "JPEG_TN";

    public IHeaders Properties
    {
        get { throw new NotSupportedException(); }
    }

    public string Title
    {
        get { throw new NotSupportedException(); }
    }

    public DlnaMime Type => DlnaMime.ImageJPEG;

    public int CompareTo(IMediaItem? other)
    {
        throw new NotSupportedException();
    }

    public Stream CreateContentStream()
    {
        return new MemoryStream(Bytes);
    }

    public bool Equals(IMediaItem? other)
    {
        throw new NotImplementedException();
    }

    public string ToComparableTitle()
    {
        throw new NotImplementedException();
    }

    public DateTime InfoDate
    {
        get
        {
            if (_file != null)
            {
                return _file.LastWriteTimeUtc;
            }
            return DateTime.Now;
        }
    }

    public long? InfoSize
    {
        get
        {
            try
            {
                var b = Bytes;
                return b?.Length;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }
        if (_bytes == null)
        {
            throw new NotSupportedException("No cover loaded");
        }
        info.AddValue("b", _bytes);
        info.AddValue("w", _width);
        info.AddValue("h", _height);
    }

    internal event EventHandler? OnCoverLazyLoaded;

    internal byte[]? ForceLoad()
    {
        try
        {
            if (_bytes == null)
            {
                var thumb = thumber.GetThumbnail(
                  _file,
                  _width,
                  _height
                  );
                _bytes = thumb.GetData();
                _height = thumb.Height;
                _width = thumb.Width;
            }
        }
        catch (NotSupportedException ex)
        {
            Logger.LogDebug(ex, "Failed to load thumb for {fileName}", _file.FullName);
        }
        catch (Exception ex)
        {
            if (!_warned)
            {
                Logger.LogWarning(ex, "Failed to load thumb for {fileName}", _file.FullName);
                _warned = true;
            }
            else
            {
                Logger.LogDebug(ex, "Failed to load thumb for {fileName}", _file.FullName);
            }
            return null;
        }
        if (_bytes == null)
        {
            _bytes = new byte[0];
        }
        OnCoverLazyLoaded?.Invoke(this, EventArgs.Empty);
        return _bytes;
    }
}