using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Media.Ui.Controls;

internal sealed class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged where TKey: notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary = new();

    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set
        {
            _dictionary[key] = value;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, key));
        }
    }

    public ICollection<TKey> Keys
        => _dictionary.Keys;

    public ICollection<TValue> Values
        => _dictionary.Values;

    public int Count
        => _dictionary.Count;

    public bool IsReadOnly
        => false;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, key));
    }

    public void Add(KeyValuePair<TKey, TValue> item)
        => Add(item.Key, item.Value);

    public void Clear()
    {
        _dictionary.Clear();
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
        => _dictionary.Contains(item);

    public bool ContainsKey(TKey key)
        => _dictionary.ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (_dictionary is IDictionary<TKey, TValue> dictionary)
        {
            dictionary.CopyTo(array, arrayIndex);
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => _dictionary.GetEnumerator();

    public bool Remove(TKey key)
    {
        bool value = _dictionary.Remove(key);
        if (value)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, key));
        }
        return value;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
        => Remove(item.Key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        => _dictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => _dictionary.GetEnumerator();
}
