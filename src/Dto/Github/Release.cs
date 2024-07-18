using System.Text.Json.Serialization;

namespace FFCmd.Dto.Github;
public record Release(
    [property: JsonPropertyName("assets")] ReleaseAsset[] Assets,
    [property: JsonPropertyName("assets_url")] Uri AssetsUrl,
    [property: JsonPropertyName("author")] AuthorClass Author,
    [property: JsonPropertyName("body")] string Body,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)][property: JsonPropertyName("body_html")] string BodyHtml,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)][property: JsonPropertyName("body_text")] string BodyText,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)][property: JsonPropertyName("discussion_url")] Uri DiscussionUrl,
    [property: JsonPropertyName("draft")] bool Draft,
    [property: JsonPropertyName("html_url")] Uri HtmlUrl,
    [property: JsonPropertyName("id")] long Id,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)][property: JsonPropertyName("mentions_count")] long? MentionsCount,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("node_id")] string NodeId,
    [property: JsonPropertyName("prerelease")] bool Prerelease,
    [property: JsonPropertyName("published_at")] DateTimeOffset? PublishedAt,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)][property: JsonPropertyName("reactions")] ReactionRollup Reactions,
    [property: JsonPropertyName("tag_name")] string TagName,
    [property: JsonPropertyName("tarball_url")] Uri TarballUrl,
    [property: JsonPropertyName("target_commitish")] string TargetCommitish,
    [property: JsonPropertyName("upload_url")] string UploadUrl,
    [property: JsonPropertyName("url")] Uri Url,
    [property: JsonPropertyName("zipball_url")] Uri ZipballUrl);
