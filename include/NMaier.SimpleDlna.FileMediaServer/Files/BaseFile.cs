using System.Globalization;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Interfaces.Metadata;
using NMaier.SimpleDlna.Server.Types;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.FileMediaServer.Files;

using CoverCache = LeastRecentlyUsedDictionary<string, Cover>;

internal class BaseFile : Logging, IMediaResource, IMetaInfo
{
    private static readonly CoverCache CoverCache = new(120);

    private readonly string _title;
    private string? _comparableTitle;

    private DateTime? _lastModified;

    private long? _length;

    private WeakReference _weakCover = new(null);

    protected BaseFile(FileServer server,
                       FileInfo file,
                       DlnaMime type,
                       DlnaMediaTypes mediaType,
                       ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        Id = string.Empty;
        if (server == null)
        {
            throw new ArgumentNullException(nameof(server));
        }
        Server = server;
        Item = file;

        _length = Item.Length;
        _lastModified = Item.LastWriteTimeUtc;

        Type = type;
        MediaType = mediaType;

        _title = System.IO.Path.GetFileNameWithoutExtension(Item.Name);
        if (string.IsNullOrEmpty(_title))
        {
            _title = Item.Name;
        }
        if (!string.IsNullOrWhiteSpace(_title))
        {
            try
            {
                _title = Uri.UnescapeDataString(_title);
            }
            catch (UriFormatException)
            {
                // no op
            }
        }
        _title = _title.StemNameBase();
    }

    protected Cover? CachedCover
    {
        get { return _weakCover.Target as Cover; }
        set
        {
            if (value != null)
            {
                CoverCache.AddAndPop(Item.FullName, value);
            }
            _weakCover = new WeakReference(value);
        }
    }

    protected FileServer Server { get; }

    internal FileInfo Item { get; set; }

    public virtual IMediaCoverResource Cover
    {
        get
        {
            CachedCover = new Cover(Item, LoggerFactory);
            return CachedCover;
        }
    }

    public string Id { get; set; }

    public DlnaMediaTypes MediaType { get; protected set; }

    public string Path => Item.FullName;

    public string PN => DlnaMaps.MainPN[Type];

    public virtual IHeaders Properties
    {
        get
        {
            var rv = new RawHeaders { { "Title", Title }, { "MediaType", MediaType.ToString() }, { "Type", Type.ToString() } };
            if (InfoSize.HasValue)
            {
                rv.Add("SizeRaw", InfoSize.Value.ToString());
                rv.Add("Size", InfoSize.Value.FormatFileSize());
            }
            rv.Add("Date", InfoDate.ToString(CultureInfo.InvariantCulture));
            rv.Add("DateO", InfoDate.ToString("o"));
            try
            {
                if (Cover != null)
                {
                    rv.Add("HasCover", "true");
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug(ex, "Failed to access CachedCover");
            }
            return rv;
        }
    }

    public virtual string Title => _title;

    public DlnaMime Type { get; protected set; }

    public virtual int CompareTo(IMediaItem? other)
    {
        if (other == null)
        {
            return 1;
        }
        return new NaturalStringComparer().Compare(_title, other.Title);
    }

    public Stream CreateContentStream()
    {
        try
        {
            return FileStreamCache.Get(Item, Logger);
        }
        catch (FileNotFoundException ex)
        {
            Logger.LogError(ex, "Failed to access: {item}", Item.FullName);
            Server.DelayedRescan(WatcherChangeTypes.Deleted);
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            Logger.LogError(ex, "Failed to access: {item}", Item.FullName);
            Server.DelayedRescan(WatcherChangeTypes.Changed);
            throw;
        }
        catch (IOException ex)
        {
            Logger.LogError(ex, "Failed to access: {item}", Item.FullName);
            Server.DelayedRescan(WatcherChangeTypes.Changed);
            throw;
        }
    }

    public bool Equals(IMediaItem? other)
    {
        if (other == null)
        {
            return false;
        }
        return new NaturalStringComparer().Equals(_title, other.Title);
    }

    public string ToComparableTitle()
    {
        return _comparableTitle ?? (_comparableTitle = Title.StemCompareBase());
    }

    public DateTime InfoDate
    {
        get
        {
            if (!_lastModified.HasValue)
            {
                _lastModified = Item.LastWriteTimeUtc;
            }
            return _lastModified.Value;
        }
    }

    public long? InfoSize
    {
        get
        {
            return _length ??= Item.Length;
        }
    }

    internal static BaseFile GetFile(PlainFolder parentFolder,
                                     FileInfo file,
                                     DlnaMime type,
                                     DlnaMediaTypes mediaType,
                                     ILoggerFactory loggerFactory)
    {
        return mediaType switch
        {
            DlnaMediaTypes.Video => new VideoFile(parentFolder.Server, file, type, loggerFactory),
            DlnaMediaTypes.Audio => new AudioFile(parentFolder.Server, file, type, loggerFactory),
            DlnaMediaTypes.Image => new ImageFile(parentFolder.Server, file, type, loggerFactory),
            _ => new BaseFile(parentFolder.Server, file, type, mediaType, loggerFactory),
        };
    }

    internal Cover? MaybeGetCover()
    {
        return CachedCover;
    }

    public virtual void LoadCover()
    {
        if (CachedCover != null)
        {
            return;
        }
        CachedCover = new Cover(Item, LoggerFactory);
        CachedCover.ForceLoad();
        CachedCover = null;
    }
}