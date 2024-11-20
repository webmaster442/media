// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Collections;

namespace Media.Database;

public sealed class DbHashSet<T> : ISet<T>, IReadOnlySet<T>, IDirtyFlag
{
    private readonly HashSet<T> _hashSet;

    public DbHashSet()
        => _hashSet = new HashSet<T>();

    public DbHashSet(IEnumerable<T> collection)
        => _hashSet = new HashSet<T>(collection);

    public DbHashSet(IEqualityComparer<T> comparer)
        => _hashSet = new HashSet<T>(comparer);

    public DbHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        => _hashSet = new HashSet<T>(collection, comparer);

    public DbHashSet(int capacity)
        => _hashSet = new HashSet<T>(capacity);

    public DbHashSet(int capacity, IEqualityComparer<T> comparer)
        => _hashSet = new HashSet<T>(capacity, comparer);

    public bool IsDirty { get; set; }

    public int Count
        => _hashSet.Count;

    public bool IsReadOnly
        => false;

    private void UdapteDirtyFlag(bool value)
    {
        if (!IsDirty && value)
            IsDirty = value;
    }

    public bool Add(T item)
    {
        bool result = _hashSet.Add(item);
        UdapteDirtyFlag(result);
        return result;
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        _hashSet.ExceptWith(other);
        UdapteDirtyFlag(true);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        _hashSet.IntersectWith(other);
        UdapteDirtyFlag(true);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
        => _hashSet.IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<T> other)
        => _hashSet.IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<T> other)
        => _hashSet.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<T> other)
        => _hashSet.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<T> other)
        => _hashSet.Overlaps(other);

    public bool SetEquals(IEnumerable<T> other)
        => _hashSet.SetEquals(other);

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        _hashSet.SymmetricExceptWith(other);
        UdapteDirtyFlag(true);
    }

    public void UnionWith(IEnumerable<T> other)
    {
        _hashSet.UnionWith(other);
        UdapteDirtyFlag(true);
    }

    void ICollection<T>.Add(T item)
        => Add(item);

    public void Clear()
    {
        UdapteDirtyFlag(Count != 0);
        _hashSet.Clear();
    }

    public bool Contains(T item)
        => _hashSet.Contains(item);

    public void CopyTo(T[] array, int arrayIndex)
        => _hashSet.CopyTo(array, arrayIndex);

    public bool Remove(T item)
    {
        bool result = _hashSet.Remove(item);
        UdapteDirtyFlag(result);
        return result;
    }

    public IEnumerator<T> GetEnumerator()
        => _hashSet.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _hashSet.GetEnumerator();
}
