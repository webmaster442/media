using System.Collections;
using System.Collections.Generic;

namespace Media.Database;

public sealed class DbList<T> : IList<T>, IReadOnlyList<T>, IDirtyFlag
{
    private readonly List<T> _list;

    public DbList()
        => _list = new List<T>();

    public DbList(IEnumerable<T> collection)
        => _list = new List<T>(collection);

    public DbList(int capacity)
        => _list = new List<T>(capacity);

    public T this[int index] 
    {
        get => _list[index];
        set
        {
            _list[index] = value;
            IsDirty = true;
        }
    }

    public int Count => _list.Count;

    public bool IsReadOnly => false;

    public bool IsDirty { get; set; }

    public void Add(T item)
    {
        _list.Add(item);
        IsDirty = true;
    }

    public void Clear()
    {
        if (_list.Count != 0)
        {
            IsDirty = true;
        }
        _list.Clear();
    }

    public bool Contains(T item)
        => _list.Contains(item);

    public void CopyTo(T[] array, int arrayIndex)
        => _list.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator()
        => _list.GetEnumerator();

    public int IndexOf(T item)
        => _list.IndexOf(item);

    public void Insert(int index, T item)
    {
        _list.Insert(index, item);
        IsDirty = true;
    }

    public bool Remove(T item)
    {
        bool result = _list.Remove(item);
        if (result)
        {
            IsDirty = true;
        }
        return result;
    }

    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
        IsDirty = true;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => _list.GetEnumerator();
}
