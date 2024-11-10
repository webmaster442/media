using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Media.Database;

public sealed class DbDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDirtyFlag where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary;

    public DbDictionary()
        => _dictionary = new Dictionary<TKey, TValue>();

    public DbDictionary(IEqualityComparer<TKey> comparer)
        => _dictionary = new Dictionary<TKey, TValue>(comparer);

    public DbDictionary(int capacity)
        => _dictionary = new Dictionary<TKey, TValue>(capacity);

    public DbDictionary(int capacity, IEqualityComparer<TKey> comparer)
        => _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);

    public DbDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        => _dictionary = new Dictionary<TKey, TValue>(collection);

    public DbDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        => _dictionary = new Dictionary<TKey, TValue>(collection, comparer);

    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set
        {
            _dictionary[key] = value;
            IsDirty = true;
        }
    }

    public ICollection<TKey> Keys
        => _dictionary.Keys;

    public ICollection<TValue> Values
        => _dictionary.Values;

    public int Count => _dictionary.Count;

    public bool IsReadOnly => false;

    public bool IsDirty { get; set; }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
        IsDirty = true;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        _dictionary.Add(item.Key, item.Value);
        IsDirty = true;
    }

    public void Clear()
    {
        _dictionary.Clear();
        IsDirty = true;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
        => _dictionary.Contains(item);

    public bool ContainsKey(TKey key)
        => _dictionary.ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        => ((IDictionary<TKey, TValue>)_dictionary).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => _dictionary.GetEnumerator();

    public bool Remove(TKey key)
    {
        bool result = _dictionary.Remove(key);
        if (result)
            IsDirty = true;
        return result;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
        => _dictionary.Remove(item.Key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        => _dictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => _dictionary.GetEnumerator();
}
