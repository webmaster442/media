﻿namespace NMaier.SimpleDlna.Server.Utilities;

public abstract class Repository<TInterface>
where TInterface : class, IRepositoryItem
{
    private readonly Dictionary<string, TInterface> items =
      BuildRepository();

    private static Dictionary<string, TInterface> BuildRepository()
    {
        var found = new Dictionary<string, TInterface>(StringComparer.CurrentCultureIgnoreCase);
        var type = typeof(TInterface).Name;
        var a = typeof(TInterface).Assembly;
        foreach (var t in a.GetTypes())
        {
            if (t.GetInterface(type) == null)
            {
                continue;
            }
            var ctor = t.GetConstructor(new Type[] { });
            if (ctor == null)
            {
                continue;
            }
            var item = ctor.Invoke(new object[] { }) as TInterface;
            if (item == null)
            {
                continue;
            }
            found.Add(item.Name, item);
        }
        return found;
    }

    public IDictionary<string, IRepositoryItem> ListItems()
    {
        return items.Values.ToDictionary<TInterface, string, IRepositoryItem>(v => v.Name, v => v);
    }

    public TInterface Lookup(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
              "Invalid repository name",
              nameof(name));
        }
        var argsplit = name.Split(new[] { ':' }, 2);
        name = argsplit[0].ToUpperInvariant().Trim();
        if (!items.TryGetValue(name, out TInterface? result))
        {
            throw new RepositoryLookupException(name);
        }
        if (argsplit.Length == 1 || !(result is IConfigurable))
        {
            return result;
        }
        var parameters = new ConfigParameters(argsplit[1]);
        if (parameters.Count == 0)
        {
            return result;
        }
        var ctor = result.GetType().GetConstructor(new Type[] { });
        if (ctor == null)
        {
            throw new RepositoryLookupException(name);
        }
        try
        {
            var item = ctor.Invoke(new object[] { }) as TInterface;
            if (item == null)
            {
                throw new RepositoryLookupException(name);
            }
            var configItem = item as IConfigurable;
            configItem?.SetParameters(parameters);
            return item;
        }
        catch (Exception ex)
        {
            throw new RepositoryLookupException($"Cannot construct repository item: {ex.Message}", ex);
        }
    }
}