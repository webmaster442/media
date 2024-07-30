using System.Collections;

namespace Media.Infrastructure;

internal class Playlist : IEnumerable<string>
{
    private readonly List<string> _items;

    public Playlist()
    {
        _items = new List<string>();
    }

    public void Clear()
        => _items.Clear();

    public void Add(string path)
    {
        var fullname = Path.GetFullPath(path);
        _items.Add(fullname);
    }

    public int AddRange(IEnumerable<string> items)
    {
        int count = 0;
        foreach (var item in items)
        {
            Add(item);
            ++count;
        }
        return count;
    }

    public int Count => _items.Count;

    public bool Remove(string path)
    {
        var fullname = Path.GetFullPath(path);
        return _items.Remove(fullname);
    }

    public IEnumerator<string> GetEnumerator()
        => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _items.GetEnumerator();
}
