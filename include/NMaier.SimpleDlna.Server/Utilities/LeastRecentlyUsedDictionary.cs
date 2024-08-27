using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NMaier.SimpleDlna.Server.Utilities;

public sealed class LeastRecentlyUsedDictionary<TKey, TValue>
  : IDictionary<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> items = new();

    private readonly LinkedList<KeyValuePair<TKey, TValue>> order =
      new LinkedList<KeyValuePair<TKey, TValue>>();

    private readonly uint toDrop;

    public LeastRecentlyUsedDictionary(uint capacity)
    {
        Capacity = capacity;
        toDrop = Math.Min(10, (uint)(capacity * 0.07));
    }

    public LeastRecentlyUsedDictionary(int capacity)
      : this((uint)capacity)
    {
    }

    public uint Capacity { get; }

    public int Count => items.Count;

    public bool IsReadOnly => false;

    public ICollection<TKey> Keys => items.Keys;

    public ICollection<TValue> Values => (from i in items.Values
                                          select i.Value.Value).ToList();

    public TValue this[TKey key]
    {
        get { return items[key].Value.Value; }
        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            Remove(key);
            Add(key, value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        AddAndPop(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Add(TKey key, TValue value)
    {
        AddAndPop(new KeyValuePair<TKey, TValue>(key, value));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Clear()
    {
        items.Clear();
        lock (order)
        {
            order.Clear();
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return items.ContainsKey(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        return items.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return items.Select(i => i.Value.Value).GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool Remove(TKey key)
    {
        if (items.TryRemove(key, out LinkedListNode<KeyValuePair<TKey, TValue>>? node))
        {
            lock (order)
            {
                order.Remove(node);
            }
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (items.TryRemove(item.Key, out LinkedListNode<KeyValuePair<TKey, TValue>>? node))
        {
            lock (order)
            {
                order.Remove(node);
            }
            return true;
        }
        return false;
    }

    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value)
    {
        if (items.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TValue>>? node))
        {
            value = node.Value.Value;
            return value != null;
        }
        value = default;
        return false;
    }

    private TValue? MaybeDropSome()
    {
        if (Count <= Capacity)
        {
            return default;
        }
        lock (order)
        {
            var rv = default(TValue);
            for (var i = 0; i < toDrop; ++i)
            {
                var last = order.Last;
                if (last != null)
                {
                    var key = last.Value.Key;
                    if (items.TryRemove(key, out LinkedListNode<KeyValuePair<TKey, TValue>>? item))
                    {
                        rv = item.Value.Value;
                    }
                    order.RemoveLast();
                }
            }
            return rv;
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public TValue? AddAndPop(KeyValuePair<TKey, TValue> item)
    {
        LinkedListNode<KeyValuePair<TKey, TValue>> node;
        lock (order)
        {
            node = order.AddFirst(item);
        }
        items.TryAdd(item.Key, node);
        return MaybeDropSome();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public TValue? AddAndPop(TKey key, TValue value)
    {
        return AddAndPop(new KeyValuePair<TKey, TValue>(key, value));
    }
}