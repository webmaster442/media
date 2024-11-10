// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Database;

/// <summary>
/// Extensions for <see cref="JsonDocumentStore"/>.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Deserialize a stored collection as a List
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    /// <param name="db">A JsonDocumentStore instance</param>
    /// <param name="key">Collection key name</param>
    /// <returns>An awaitable task</returns>
    public static async Task<DbList<T>> DeserializeCollectionAsList<T>(this JsonDocumentStore db, string key) where T : notnull
    {
        var (_, length) = await db.GetCollectionCount(key);
        DbList<T> result = new(length);
        await foreach (var item in db.DeserializeCollection<T>(key))
        {
            result.Add(item);
        }
        result.IsDirty = false;
        return result;
    }

    /// <summary>
    /// Deserialize a stored collection as a HashSet
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    /// <param name="db">A JsonDocumentStore instance</param>
    /// <param name="key">Collection key name</param>
    /// <returns>An awaitable task</returns>
    public static async Task<DbHashSet<T>> DeserializeCollectionAsHashSet<T>(this JsonDocumentStore db, string key) where T : notnull
    {
        var (_, length) = await db.GetCollectionCount(key);
        DbHashSet<T> result = new(length);
        await foreach (var item in db.DeserializeCollection<T>(key))
        {
            result.Add(item);
        }
        result.IsDirty = false;
        return result;
    }

    /// <summary>
    /// Deserialize a stored collection as a Dictironary
    /// </summary>
    /// <typeparam name="TKey">Type of keys in the dictionary</typeparam>
    /// <typeparam name="TValue">Type of values in the dictionary</typeparam>
    /// <param name="db">A JsonDocumentStore instance</param>
    /// <param name="key">Collection key name</param>
    /// <returns>An awaitable task</returns>
    public static async Task<DbDictionary<TKey, TValue>> DeserializeCollectionAsDictionary<TKey, TValue>(this JsonDocumentStore db, string key) where TKey : notnull
    {
        var (_, length) = await db.GetCollectionCount(key);
        DbDictionary<TKey, TValue> result = new(length);
        await foreach (var item in db.DeserializeCollection<KeyValuePair<TKey, TValue>>(key))
        {
            result.Add(item.Key, item.Value);
        }
        result.IsDirty = false;
        return result;
    }
}