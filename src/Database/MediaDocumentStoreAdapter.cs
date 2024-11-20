// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Database;

internal sealed class MediaDocumentStoreAdapter : DocumentStoreAdapter
{
    private const string PlayedKey = "played";

    public DbHashSet<string> PlayedFiles { get; private set; }

    public MediaDocumentStoreAdapter()
    {
        PlayedFiles = new DbHashSet<string>();
    }

    public override async Task Init()
    {
        PlayedFiles = await _store.DeserializeCollectionAsHashSet<string>(PlayedKey);
    }

    public override async Task Save()
    {
        await _store.SerializeCollection(PlayedKey, PlayedFiles);
    }
}
