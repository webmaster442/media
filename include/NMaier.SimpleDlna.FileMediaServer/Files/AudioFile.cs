using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

using TagLib;

using File = TagLib.File;
using NMaier.SimpleDlna.FileMediaServer.Files;
using Microsoft.Extensions.Logging;

namespace NMaier.SimpleDlna.FileMediaServer.Files;

internal sealed class AudioFile
  : BaseFile, IMediaAudioResource
{
    private static readonly TimeSpan emptyDuration = new TimeSpan(0);
    private string? _album;

    private string? _artist;

    private string? _description;

    private TimeSpan? _duration;

    private string? _genre;

    private bool _initialized;

    private string? _performer;

    private string? _title;

    private int? _track;

    internal AudioFile(FileServer server, FileInfo aFile, DlnaMime aType, ILoggerFactory loggerFactory)
      : base(server, aFile, aType, DlnaMediaTypes.Audio, loggerFactory)
    {
    }

    public override IMediaCoverResource Cover
    {
        get
        {
            if (CachedCover == null)
            {
                MaybeInit();
            }
            return CachedCover ?? new Cover(base.Item, LoggerFactory);
        }
    }

    public string MetaAlbum
    {
        get
        {
            MaybeInit();
            return _album ?? string.Empty;
        }
    }

    public string MetaArtist
    {
        get
        {
            MaybeInit();
            return _artist ?? string.Empty;
        }
    }

    public string MetaDescription
    {
        get
        {
            MaybeInit();
            return _description ?? string.Empty;
        }
    }

    public TimeSpan? MetaDuration
    {
        get
        {
            MaybeInit();
            return _duration;
        }
    }

    public string MetaGenre
    {
        get
        {
            MaybeInit();
            return _genre ?? string.Empty;
        }
    }

    public string MetaPerformer
    {
        get
        {
            MaybeInit();
            return _performer ?? string.Empty;
        }
    }

    public int? MetaTrack
    {
        get
        {
            MaybeInit();
            return _track;
        }
    }

    public override IHeaders Properties
    {
        get
        {
            MaybeInit();
            var rv = base.Properties;
            if (_album != null)
            {
                rv.Add("Album", _album);
            }
            if (_artist != null)
            {
                rv.Add("Artist", _artist);
            }
            if (_description != null)
            {
                rv.Add("Description", _description);
            }
            if (_duration != null)
            {
                rv.Add("Duration", _duration.Value.ToString("g"));
            }
            if (_genre != null)
            {
                rv.Add("Genre", _genre);
            }
            if (_performer != null)
            {
                rv.Add("Performer", _performer);
            }
            if (_track != null)
            {
                rv.Add("Track", _track.Value.ToString());
            }
            return rv;
        }
    }

    public override string Title
    {
        get
        {
            MaybeInit();
            if (!string.IsNullOrWhiteSpace(_title))
            {
                if (_track.HasValue)
                {
                    return $"{_track.Value:D2}. — {_title}";
                }
                return _title;
            }
            return base.Title;
        }
    }

    public override int CompareTo(IMediaItem? other)
    {
        if (_track.HasValue)
        {
            var oa = other as AudioFile;
            int rv;
            if (oa?._track != null && (rv = _track.Value.CompareTo(oa._track.Value)) != 0)
            {
                return rv;
            }
        }
        return base.CompareTo(other);
    }

    private void InitCover(Tag tag)
    {
        IPicture? pic = null;
        foreach (var p in tag.Pictures)
        {
            if (p.Type == PictureType.FrontCover)
            {
                pic = p;
                break;
            }
            switch (p.Type)
            {
                case PictureType.Other:
                case PictureType.OtherFileIcon:
                case PictureType.FileIcon:
                    pic = p;
                    break;

                default:
                    if (pic == null)
                    {
                        pic = p;
                    }
                    break;
            }
        }
        if (pic != null)
        {
            try
            {
                CachedCover = new Cover(Item, pic.Data.ToStream(), LoggerFactory);
            }
            catch (Exception ex)
            {
                Logger.LogDebug(ex, "Failed to generate thumb for {path}", Item.FullName);
            }
        }
    }

    private void MaybeInit()
    {
        if (_initialized)
        {
            return;
        }

        try
        {
            using (var tl = File.Create(new TagLibFileAbstraction(Item)))
            {
                try
                {
                    _duration = tl.Properties.Duration;
                    if (_duration.Value.TotalSeconds < 0.1)
                    {
                        _duration = null;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, "Failed to transpose Properties props");
                }

                try
                {
                    var t = tl.Tag;
                    SetProperties(t);
                    InitCover(t);
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, "Failed to transpose Tag props");
                }
            }

            _initialized = true;
        }
        catch (CorruptFileException ex)
        {
            Logger.LogDebug(ex, "Failed to read meta data via taglib for file {filename}", Item.FullName);
            _initialized = true;
        }
        catch (UnsupportedFormatException ex)
        {
            Logger.LogDebug(ex, "Failed to read meta data via taglib for file {filename}", Item.FullName);
            _initialized = true;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Unhandled exception reading meta data for file {filename}", Item.FullName);
        }
    }

    private void SetProperties(Tag tag)
    {
        _genre = tag.FirstGenre;
        if (string.IsNullOrWhiteSpace(_genre))
        {
            _genre = null;
        }

        if (tag.Track != 0 && tag.Track < 1 << 10)
        {
            _track = (int)tag.Track;
        }

        _title = tag.Title;
        if (string.IsNullOrWhiteSpace(_title))
        {
            _title = null;
        }

        _description = tag.Comment;
        if (string.IsNullOrWhiteSpace(_description))
        {
            _description = null;
        }

        _performer = string.IsNullOrWhiteSpace(_artist) ? tag.JoinedPerformers : tag.JoinedPerformersSort;
        if (string.IsNullOrWhiteSpace(_performer))
        {
            _performer = null;
        }

        _artist = tag.JoinedAlbumArtists;
        if (string.IsNullOrWhiteSpace(_artist))
        {
            _artist = tag.JoinedComposers;
        }
        if (string.IsNullOrWhiteSpace(_artist))
        {
            _artist = null;
        }

        _album = tag.AlbumSort;
        if (string.IsNullOrWhiteSpace(_album))
        {
            _album = tag.Album;
        }
        if (string.IsNullOrWhiteSpace(_album))
        {
            _album = null;
        }
    }

    public override void LoadCover()
    {
        // No op
    }
}