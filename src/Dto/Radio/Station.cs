// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Media.Dto.Radio;

public record Station([property: JsonPropertyName("name")] string Name,
                      [property: JsonPropertyName("url")] string Url,
                      [property: JsonPropertyName("url_resolved")] string UrlResolved,
                      [property: JsonPropertyName("homepage")] string Homepage,
                      [property: JsonPropertyName("favicon")] string Favicon,
                      [property: JsonPropertyName("tags")] string Tags,
                      [property: JsonPropertyName("country")] string Country,
                      [property: JsonPropertyName("countrycode")] string Countrycode);
