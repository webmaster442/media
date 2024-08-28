using System.Reflection;

using NMaier.SimpleDlna.Server.Types;
using NMaier.SimpleDlna.Server.Utilities;
using NMaier.SimpleDlna.Thumbnails;

namespace NMaier.SimpleDlna.FileMediaServer;

public sealed class ThumbnailMaker : Logging
{
    private static readonly LeastRecentlyUsedDictionary<string, CacheItem> cache =
      new LeastRecentlyUsedDictionary<string, CacheItem>(1 << 11);

    private static readonly Dictionary<DlnaMediaTypes, List<IThumbnailLoader>> thumbers =
      BuildThumbnailers();

    private static Dictionary<DlnaMediaTypes, List<IThumbnailLoader>> BuildThumbnailers()
    {
        var types = Enum.GetValues(typeof(DlnaMediaTypes));
        var buildThumbnailers = types.Cast<DlnaMediaTypes>().ToDictionary(i => i, i => new List<IThumbnailLoader>());
        var a = Assembly.GetExecutingAssembly();
        foreach (var t in a.GetTypes())
        {
            if (t.GetInterface("IThumbnailLoader") == null)
            {
                continue;
            }
            var ctor = t.GetConstructor(new Type[] { });
            var thumber = ctor?.Invoke(new object[] { }) as IThumbnailLoader;
            if (thumber == null)
            {
                continue;
            }
            foreach (DlnaMediaTypes i in types)
            {
                if (thumber.Handling.HasFlag(i))
                {
                    buildThumbnailers[i].Add(thumber);
                }
            }
        }
        return buildThumbnailers;
    }

    private static bool GetThumbnailFromCache(ref string key, ref int width,
      ref int height, out byte[] rv)
    {
        key = $"{width}x{height} {key}";
        lock (cache)
        {
            CacheItem ci;
            if (cache.TryGetValue(key, out ci))
            {
                rv = ci.Data;
                width = ci.Width;
                height = ci.Height;
                return true;
            }
        }
        rv = null;
        return false;
    }

    private byte[] GetThumbnailInternal(string key, object item,
      DlnaMediaTypes type, ref int width,
      ref int height)
    {
        var thumbnailers = thumbers[type];
        var rw = width;
        var rh = height;
        foreach (var thumber in thumbnailers)
        {
            try
            {
                using (var i = thumber.GetThumbnail(item, ref width, ref height))
                {
                    var rv = i.ToArray();
                    lock (cache)
                    {
                        cache[key] = new CacheItem(rv, rw, rh);
                    }
                    return rv;
                }
            }
            catch (Exception ex)
            {
                Debug($"{thumber.GetType()} failed to thumbnail a resource", ex);
            }
        }
        throw new ArgumentException("Not a supported resource");
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
        byte[] rv;
        if (GetThumbnailFromCache(ref key, ref width, ref height, out rv))
        {
            return new Thumbnail(width, height, rv);
        }

        rv = GetThumbnailInternal(key, file, mediaType, ref width, ref height);
        return new Thumbnail(width, height, rv);
    }

    public IThumbnail GetThumbnail(string key, DlnaMediaTypes type,
      Stream stream, int width, int height)
    {
        byte[] rv;
        if (GetThumbnailFromCache(ref key, ref width, ref height, out rv))
        {
            return new Thumbnail(width, height, rv);
        }
        rv = GetThumbnailInternal(key, stream, type, ref width, ref height);
        return new Thumbnail(width, height, rv);
    }

    private struct CacheItem
    {
        public readonly byte[] Data;

        public readonly int Height;

        public readonly int Width;

        public CacheItem(byte[] aData, int aWidth, int aHeight)
        {
            Data = aData;
            Width = aWidth;
            Height = aHeight;
        }
    }
}
