// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Database;

internal sealed class MediaDocumentStore
{
    private readonly JsonDocumentStore _store;

    private const string PlayedKey = "played";
    public HashSet<string> PlayedFiles { get; private set; }

    public MediaDocumentStore()
    {
        var file = Path.Combine(AppContext.BaseDirectory, "mediadb.zip");
        _store = new JsonDocumentStore(file, new JsonDocumentStoreOptions
        {
            ReturnEmptyCollectionOnFileNotFound = true,
        });
        PlayedFiles = new HashSet<string>();
    }

    public async Task Init()
    {
        PlayedFiles = await _store.DeserializeCollectionAsHashSet<string>(PlayedKey);
    }

    public async Task Save()
    {
        await _store.SerializeCollection(PlayedKey, PlayedFiles);
    }
}
