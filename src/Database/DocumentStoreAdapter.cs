// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Database;

internal abstract class DocumentStoreAdapter
{
    public abstract Task Init();
    public abstract Task Save();

    protected readonly JsonDocumentStore _store;

    public DocumentStoreAdapter()
    {
        var file = Path.Combine(AppContext.BaseDirectory, "mediadb.zip");
        _store = new JsonDocumentStore(file, new JsonDocumentStoreOptions
        {
            ReturnEmptyCollectionOnFileNotFound = true,
        });
    }
}
