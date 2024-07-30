// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Media.Dto.Github;

public record ReleaseAsset(
    [property: JsonPropertyName("browser_download_url")] Uri BrowserDownloadUrl,
    [property: JsonPropertyName("content_type")] string ContentType,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("download_count")] long DownloadCount,
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("node_id")] string NodeId,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("state")] State State,
    [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt,
    [property: JsonPropertyName("uploader")] SimpleUser Uploader,
    [property: JsonPropertyName("url")] Uri Url);
