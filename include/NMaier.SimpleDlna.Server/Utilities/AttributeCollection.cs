using System.Collections;

namespace NMaier.SimpleDlna.Server.Utilities;

using Attribute = KeyValuePair<string, string?>;

public class AttributeCollection : IEnumerable<Attribute>
{
    private readonly IList<Attribute> _list = new List<Attribute>();

    public int Count => _list.Count;

    public ICollection<string> Keys => (from i in _list
                                        select i.Key).ToList();

    public ICollection<string> Values => (from i in _list
                                          select i.Value).ToList();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public IEnumerator<Attribute> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public void Add(Attribute item)
    {
        _list.Add(item);
    }

    public void Add(string key, string? value)
    {
        _list.Add(new Attribute(key, value));
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(Attribute item)
    {
        return _list.Contains(item);
    }

    public bool Has(string key)
    {
        return Has(key, StringComparer.CurrentCultureIgnoreCase);
    }

    public bool Has(string key, StringComparer comparer)
    {
        return _list.Any(e => comparer.Equals(key, e.Key));
    }

    public IEnumerable<string> GetValuesForKey(string key)
    {
        return GetValuesForKey(key, StringComparer.CurrentCultureIgnoreCase);
    }

    public IEnumerable<string> GetValuesForKey(string key, StringComparer comparer)
    {
        return from i in _list
               where comparer.Equals(i.Key, key)
               select i.Value;
    }
}