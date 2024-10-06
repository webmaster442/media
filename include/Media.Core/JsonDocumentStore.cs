using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Media.Core;

/// <summary>
/// A simple document store that uses a zip file as a backing store.
/// </summary>
public sealed class JsonDocumentStore
{
    private readonly string _zipFile;
    private readonly JsonSerializerOptions _options;
    private const string ControlDataPrefix = "control";
    private const string BlobPrefix = "blob";
    private const int ChunkSize = 25;

    private record class CollectionInfo(int Chunks, int Count);

    /// <summary>
    /// Creates a new instance of the JsonDocumentStore class.
    /// </summary>
    /// <param name="zipFile">backing zip file path</param>
    /// <param name="additionalConverters">additional JSON converters</param>
    public JsonDocumentStore(string zipFile, params JsonConverter[] additionalConverters)
    {
        _options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            IgnoreReadOnlyProperties = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
            Converters =
            {
                new JsonStringEnumConverter(),
            },
        };
        foreach (var additional in additionalConverters)
        {
            _options.Converters.Add(additional);
        }
        _zipFile = zipFile;
    }
    /// <summary>
    /// Store an object in the document store.
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="key">Object key</param>
    /// <param name="obj">object instance</param>
    /// <returns>an awaitable task</returns>
    public async Task SerializeAsync<T>(string key,
                                        T obj)
        where T : class
    {
        using (var zip = ZipFile.Open(_zipFile, ZipArchiveMode.Update))
        {
            var entry = zip.GetEntry(key);
            entry?.Delete();
            entry = zip.CreateEntry(key);
            await using (var stream = entry.Open())
            {
                await JsonSerializer.SerializeAsync(stream, obj, _options);
            }
        }
    }

    /// <summary>
    /// Deserialize an object from the document store.
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="key">Object key</param>
    /// <returns>an awaitable task</returns>
    public async Task<T?> DeserializeAsync<T>(string key)
        where T : class
    {
        using (var zip = ZipFile.Open(_zipFile, ZipArchiveMode.Read))
        {
            var entry = zip.GetEntry(key);
            if (entry == null)
            {
                return null;
            }
            await using (var stream = entry.Open())
            {
                return await JsonSerializer.DeserializeAsync<T>(stream, _options);
            }
        }
    }

    /// <summary>
    /// Serialize a collection of items to the document store.
    /// </summary>
    /// <typeparam name="T">Collection item typer</typeparam>
    /// <param name="key">Collection key</param>
    /// <param name="items">items to serialize</param>
    /// <returns>an awaitable task</returns>
    public async Task SerializeCollection<T>(string key,
                                             ICollection<T> items)
    {
        async Task WriteChunk<T>(ZipArchive zip, string key, int chunks, List<T> currentChunk)
        {
            var entryKey = $"{key}\\{chunks}";
            var entry = zip.CreateEntry(entryKey);
            await using (var stream = entry.Open())
            {
                await JsonSerializer.SerializeAsync(stream, currentChunk, _options);
            }
        }

        using (var zip = ZipFile.Open(_zipFile, ZipArchiveMode.Update))
        {
            int counter = 0;
            int chunks = 0;
            await CleanupCollectionItems(zip, key);

            List<T> currentChunk = new List<T>(ChunkSize);
            foreach (var item in items)
            {
                if (currentChunk.Count< ChunkSize)
                {
                    currentChunk.Add(item);
                    ++counter;
                }
                else
                {
                    await WriteChunk(zip, key, chunks, currentChunk);
                    currentChunk.Clear();
                    ++chunks;
                }
            }
            if (currentChunk.Count > 0)
                await WriteChunk(zip, key, chunks, currentChunk);

            await SetCollectionInfo(zip, key, chunks, counter);
        }
    }

    /// <summary>
    /// Deserialize a collection of items from the document store.
    /// </summary>
    /// <typeparam name="T">Collection item typer</typeparam>
    /// <param name="key">Collection key</param>
    /// <returns>An async enumerable of the items</returns>
    public async IAsyncEnumerable<T> DeserializeCollection<T>(string key) where T : notnull
    {
        using (var zip = ZipFile.Open(_zipFile, ZipArchiveMode.Read))
        {
            var info = await GetCollectionInfo(zip, key);
            for (int i=0; i<info.Chunks; i++)
            {
                var entryKey = $"{key}\\{i}";
                var entry = zip.GetEntry(entryKey);
                if (entry != null)
                {
                    await using (var stream = entry.Open())
                    {
                        T[] chunk = await JsonSerializer.DeserializeAsync<T[]>(stream, _options)
                            ?? throw new IOException("Corrupted file");
                        foreach (var item in chunk)
                        {
                            yield return item;
                        }
                    }
                }

            }
        }
    }

    /// <summary>
    /// Get a blob stream.
    /// </summary>
    /// <param name="key">blob stream key</param>
    /// <returns>Blob stream</returns>
    public Stream? GetBlobStream(string key)
    {
        var entryKey = $"{BlobPrefix}\\{key}";
        using (var zip = ZipFile.Open(_zipFile, ZipArchiveMode.Read))
        {
            if (zip.GetEntry(entryKey) is ZipArchiveEntry entry)
            {
                return entry.Open();
            }
        }
        return null;
    }

    /// <summary>
    /// Allows writing to a blob stream.
    /// </summary>
    /// <param name="key">blob stream key</param>
    /// <param name="stream">stream to write</param>
    /// <returns>an awitable task</returns>
    public async Task SetBlobStream(string key, Stream stream)
    {
        using (var zip = ZipFile.Open(_zipFile, ZipArchiveMode.Update))
        {
            var entryKey = $"{BlobPrefix}\\{key}";
            if (zip.GetEntry(entryKey) is ZipArchiveEntry entry)
            {
                entry.Delete();
            }
            entry = zip.CreateEntry(entryKey);
            await using (var entryStream = entry.Open())
            {
                await stream.CopyToAsync(entryStream);
            }
        }
    }

    /// <summary>
    /// Return a count of items in a collection.
    /// </summary>
    /// <param name="key">collection key</param>
    /// <returns>collection item count</returns>
    public async Task<(int chunks, int length)> GetCollectionCount(string key)
    {
        using var zip = ZipFile.Open(_zipFile, ZipArchiveMode.Read);
        var info = await GetCollectionInfo(zip, key);
        return (info.Chunks, info.Count);
    }

    private async Task<CollectionInfo> GetCollectionInfo(ZipArchive zip, string collectionKey)
    {
        string entryKey = $"{ControlDataPrefix}\\count_{collectionKey}";
        var entry = zip.GetEntry(entryKey);
        if (entry == null)
        {
            return new CollectionInfo(0, 0);
        }
        await using var stream = entry.Open();
        return await JsonSerializer.DeserializeAsync<CollectionInfo>(stream, _options) 
            ?? throw new IOException("File is corrupted");
    }

    private async Task SetCollectionInfo(ZipArchive zip, string collectionKey, int chunks, int count)
    {
        string entryKey = $"{ControlDataPrefix}\\count_{collectionKey}";
        var entry = zip.GetEntry(entryKey);
        entry?.Delete();
        entry = zip.CreateEntry(entryKey);
        await using var stream = entry.Open();
        await JsonSerializer.SerializeAsync(stream, new CollectionInfo(chunks, count), _options);
    }

    private async Task CleanupCollectionItems(ZipArchive zip, string key)
    {
        var collectionInfo = await GetCollectionInfo(zip, key);
        if (collectionInfo.Chunks > 0)
        {
            for (uint i = 0; i < collectionInfo.Chunks; i++)
            {
                var entryKey = $"{key}\\{i}";
                var entry = zip.GetEntry(entryKey);
                entry?.Delete();
            }
        }
        await SetCollectionInfo(zip, key, 0, 0);
    }
}