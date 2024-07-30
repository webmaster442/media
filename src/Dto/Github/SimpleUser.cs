// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Media.Dto.Github;

public record SimpleUser(
    [property: JsonPropertyName("avatar_url")] Uri AvatarUrl,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("events_url")] string EventsUrl,
    [property: JsonPropertyName("followers_url")] Uri FollowersUrl,
    [property: JsonPropertyName("following_url")] string FollowingUrl,
    [property: JsonPropertyName("gists_url")] string GistsUrl,
    [property: JsonPropertyName("gravatar_id")] string GravatarId,
    [property: JsonPropertyName("html_url")] Uri HtmlUrl,
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("node_id")] string NodeId,
    [property: JsonPropertyName("organizations_url")] Uri OrganizationsUrl,
    [property: JsonPropertyName("received_events_url")] Uri ReceivedEventsUrl,
    [property: JsonPropertyName("repos_url")] Uri ReposUrl,
    [property: JsonPropertyName("site_admin")] bool SiteAdmin,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)][property: JsonPropertyName("starred_at")] string StarredAt,
    [property: JsonPropertyName("starred_url")] string StarredUrl,
    [property: JsonPropertyName("subscriptions_url")] Uri SubscriptionsUrl,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("url")] Uri Url);
