using System.Runtime.Serialization;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

using TagLib;

using File = TagLib.File;

namespace NMaier.SimpleDlna.FileMediaServer;

[Serializable]
internal sealed class VideoFile
  : BaseFile, IMediaVideoResource, ISerializable, IBookmarkable
{
    private static readonly TimeSpan emptyDuration = new TimeSpan(0);
    private string[]? actors;

    private long? bookmark;

    private string? description;

    private string? director;

    private TimeSpan? duration;

    private string? genre;

    private int? height;

    private bool initialized;

    private Subtitle? subTitle;

    private string? title;

    private int? width;

    internal VideoFile(FileServer server, FileInfo aFile, DlnaMime aType)
      : base(server, aFile, aType, DlnaMediaTypes.Video)
    {
    }

    public long? Bookmark
    {
        get { return bookmark; }
        set
        {
            bookmark = value;
        }
    }

    public IEnumerable<string> MetaActors
    {
        get
        {
            MaybeInit();
            return actors ?? Enumerable.Empty<string>();
        }
    }

    public string MetaDescription
    {
        get
        {
            MaybeInit();
            return description ?? string.Empty;
        }
    }

    public string MetaDirector
    {
        get
        {
            MaybeInit();
            return director ?? string.Empty;
        }
    }

    public TimeSpan? MetaDuration
    {
        get
        {
            MaybeInit();
            return duration;
        }
    }

    public string MetaGenre
    {
        get
        {
            MaybeInit();
            if (string.IsNullOrWhiteSpace(genre))
            {
                throw new NotSupportedException();
            }
            return genre;
        }
    }

    public int? MetaHeight
    {
        get
        {
            MaybeInit();
            return height;
        }
    }

    public int? MetaWidth
    {
        get
        {
            MaybeInit();
            return width;
        }
    }

    public override IHeaders Properties
    {
        get
        {
            MaybeInit();
            var rv = base.Properties;
            if (description != null)
            {
                rv.Add("Description", description);
            }
            if (actors != null && actors.Length != 0)
            {
                rv.Add("Actors", string.Join(", ", actors));
            }
            if (director != null)
            {
                rv.Add("Director", director);
            }
            if (duration != null)
            {
                rv.Add("Duration", duration.Value.ToString("g"));
            }
            if (genre != null)
            {
                rv.Add("Genre", genre);
            }
            if (width != null && height != null)
            {
                rv.Add(
                  "Resolution",
                  $"{width.Value}x{height.Value}"
                  );
            }
            return rv;
        }
    }

    public Subtitle Subtitle
    {
        get
        {
            try
            {
                if (subTitle == null)
                {
                    subTitle = new Subtitle(Item);
                }
            }
            catch (Exception ex)
            {
                Error("Failed to look up subtitle", ex);
                subTitle = new Subtitle();
            }
            return subTitle;
        }
    }

    public override string Title
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                return $"{base.Title} — {title}";
            }
            return base.Title;
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }
        MaybeInit();
        info.AddValue("a", actors, typeof(string[]));
        info.AddValue("de", description);
        info.AddValue("di", director);
        info.AddValue("g", genre);
        info.AddValue("t", title);
        info.AddValue("w", width);
        info.AddValue("h", height);
        info.AddValue("b", bookmark);
        info.AddValue("du", duration.GetValueOrDefault(emptyDuration).Ticks);
        info.AddValue("st", subTitle);
    }

    private void MaybeInit()
    {
        if (initialized)
        {
            return;
        }

        try
        {
            using (var tl = File.Create(new TagLibFileAbstraction(Item)))
            {
                try
                {
                    duration = tl.Properties.Duration;
                    if (duration.Value.TotalSeconds < 0.1)
                    {
                        duration = null;
                    }
                    width = tl.Properties.VideoWidth;
                    height = tl.Properties.VideoHeight;
                }
                catch (Exception ex)
                {
                    Debug("Failed to transpose Properties props", ex);
                }

                try
                {
                    var t = tl.Tag;
                    genre = t.FirstGenre;
                    title = t.Title;
                    description = t.Comment;
                    director = t.FirstComposerSort;
                    if (string.IsNullOrWhiteSpace(director))
                    {
                        director = t.FirstComposer;
                    }
                    actors = t.PerformersSort;
                    if (actors == null || actors.Length == 0)
                    {
                        actors = t.PerformersSort;
                        if (actors == null || actors.Length == 0)
                        {
                            actors = t.Performers;
                            if (actors == null || actors.Length == 0)
                            {
                                actors = t.AlbumArtists;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug("Failed to transpose Tag props", ex);
                }
            }

            initialized = true;
        }
        catch (CorruptFileException ex)
        {
            Debug(
              "Failed to read meta data via taglib for file " + Item.FullName, ex);
            initialized = true;
        }
        catch (UnsupportedFormatException ex)
        {
            Debug(
              "Failed to read meta data via taglib for file " + Item.FullName, ex);
            initialized = true;
        }
        catch (Exception ex)
        {
            Warn(
              "Unhandled exception reading meta data for file " + Item.FullName,
              ex);
        }
    }
}
