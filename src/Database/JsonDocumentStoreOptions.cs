// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Media.Database;

/// <summary>
/// Options for the JsonDocumentStore
/// </summary>
public sealed class JsonDocumentStoreOptions
{
    /// <summary>
    /// When set to true, instead of generating exceptions empty collections will be returned.
    /// </summary>
    public bool ReturnEmptyCollectionOnFileNotFound { get; set; }

    /// <summary>
    /// Additional JSON converters to use.
    /// </summary>
    public IList<JsonConverter> AdditionalConverters { get; set; }

    public JsonDocumentStoreOptions()
    {
        ReturnEmptyCollectionOnFileNotFound = false;
        AdditionalConverters = Array.Empty<JsonConverter>();
    }
}
