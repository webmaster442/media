using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Comparers;
using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Utilities;
using NMaier.SimpleDlna.Server.Views;

namespace NMaier.SimpleDlna.Server.Types;

public sealed class Identifiers : Logging
{
    public const string GENERAL_ROOT = "0";

    public const string SAMSUNG_AUDIO = "A";

    public const string SAMSUNG_IMAGES = "I";

    public const string SAMSUNG_VIDEO = "V";

    private readonly IItemComparer comparer;
    private readonly List<IFilteredView> filters = new List<IFilteredView>();

    // ReSharper disable once CollectionNeverQueried.Local
    private readonly Dictionary<string, IMediaItem> hardRefs =
      new Dictionary<string, IMediaItem>();

    private readonly Dictionary<string, WeakReference> ids =
      new Dictionary<string, WeakReference>();

    private readonly bool order;

    private readonly List<IView> views = new List<IView>();

    private Dictionary<string, string> paths =
      new Dictionary<string, string>();

    private readonly ViewRepository _viewRepository = new();

    public Identifiers(IItemComparer comparer, ILoggerFactory loggerFactory, bool order) : base(loggerFactory)
    {
        this.comparer = comparer;
        this.order = order;
    }

    public bool HasViews => views.Count != 0;

    public IEnumerable<WeakReference> Resources => (from i in ids.Values
                                                    where i.Target is IMediaResource
                                                    select i).ToList();

    private void RegisterFolderTree(IMediaFolder folder)
    {
        foreach (var f in folder.ChildFolders)
        {
            RegisterFolderTree(f);
        }
        foreach (var i in folder.ChildItems)
        {
            RegisterPath(i);
        }
        RegisterPath(folder);
    }

    private void RegisterPath(IMediaItem item)
    {
        var path = item.Path;
        string id;
        if (!paths.ContainsKey(path))
        {
            id = GenerateId(path);
            /*while (ids.ContainsKey(id = Random.Shared.Next(1000, int.MaxValue).ToString("X8")))
            {
            }*/
            paths[path] = id;
        }
        else
        {
            id = paths[path];
        }
        ids[id] = new WeakReference(item);

        item.Id = id;
    }

    private static string GenerateId(string path)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(path));
        return Convert.ToHexString(bytes);
    }

    public void AddView(string name)
    {
        try
        {
            var view = _viewRepository.Lookup(name);
            views.Add(view);
            var filter = view as IFilteredView;
            if (filter != null)
            {
                filters.Add(filter);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to add view");
            throw;
        }
    }

    public void Cleanup()
    {
        GC.Collect();
        var pc = paths.Count;
        var ic = ids.Count;
        var npaths = new Dictionary<string, string>();
        foreach (var p in paths)
        {
            if (ids[p.Value].Target == null)
            {
                ids.Remove(p.Value);
            }
            else
            {
                npaths.Add(p.Key, p.Value);
            }
        }
        paths = npaths;
        Logger.LogDebug("Cleanup complete: ids (evicted) {count} ({removedids}), paths {paths} ({removed})", ids.Count, ic - ids.Count, paths.Count,
                    pc - paths.Count);
    }

    public IMediaItem? GetItemById(string id)
    {
        return ids[id].Target as IMediaItem;
    }

    public IMediaItem? GetItemByPath(string path)
    {
        if (!paths.TryGetValue(path, out string? id))
        {
            return null;
        }
        return GetItemById(id);
    }

    public IMediaFolder RegisterFolder(string id, IMediaFolder item)
    {
        var rv = item;
        RegisterFolderTree(rv);
        foreach (var v in views)
        {
            rv = v.Transform(rv);
            RegisterFolderTree(rv);
        }
        rv.Cleanup();
        ids[id] = new WeakReference(rv);
        hardRefs[id] = rv;
        rv.Id = id;
        rv.Sort(comparer, order);
        return rv;
    }

    public bool Allowed(IMediaResource item)
    {
        return filters.All(f => f.Allowed(item));
    }
}