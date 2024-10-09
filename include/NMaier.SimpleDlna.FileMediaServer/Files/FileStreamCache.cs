using System.Diagnostics;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Utilities;

using Timer = System.Timers.Timer;

namespace NMaier.SimpleDlna.FileMediaServer.Files;

internal static class FileStreamCache
{
    private static readonly Ticker ticker = new Ticker();

    private static readonly LeastRecentlyUsedDictionary<string, CacheItem> streams =
      new LeastRecentlyUsedDictionary<string, CacheItem>(15);

    private static void Expire()
    {
        lock (streams)
        {
            foreach (var item in streams.ToArray())
            {
                var diff = item.Value.InsertionPoint - DateTime.UtcNow;
                if (diff.TotalSeconds > 5)
                {
                    item.Value.Stream?.Kill();
                    streams.Remove(item.Key);
                }
            }
        }
    }

    internal static void Clear()
    {
        lock (streams)
        {
            foreach (var item in streams)
            {
                item.Value.Stream.Kill();
            }
            streams.Clear();
        }
    }

    internal static FileReadStream Get(FileInfo info, ILogger logger)
    {
        var key = info.FullName;
        lock (streams)
        {
            CacheItem? rv;
            if (streams.TryGetValue(key, out rv))
            {
                streams.Remove(key);
                Debug.WriteLine("Retrieved file stream {0} from cache", key);
                return rv.Stream;
            }
        }
        Debug.WriteLine("Constructing file stream {0}", key);
        return new FileReadStream(info, logger);
    }

    internal static void Recycle(FileReadStream stream)
    {
        var key = stream.Name;
        lock (streams)
        {
            CacheItem? ignore;
            if (!streams.TryGetValue(key, out ignore) ||
                Equals(ignore.Stream, stream))
            {
                Debug.WriteLine("Recycling {0}", key);
                stream.Seek(0, SeekOrigin.Begin);
                var removed = streams.AddAndPop(key, new CacheItem(stream));
                removed?.Stream.Kill();
                return;
            }
        }

        stream.Kill();
    }

    private class Ticker : Timer
    {
        public Ticker() : base(10000)
        {
            Elapsed += (sender, args) => Expire();
        }
    }

    private class CacheItem
    {
        public readonly DateTime InsertionPoint = DateTime.UtcNow;
        public readonly FileReadStream Stream;

        public CacheItem(FileReadStream stream)
        {
            Stream = stream;
        }
    }
}