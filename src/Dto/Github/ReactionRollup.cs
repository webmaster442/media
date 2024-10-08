﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Media.Dto.Github;

public record ReactionRollup(
    [property: JsonPropertyName("+1")] long The1,
    [property: JsonPropertyName("-1")] long ReactionRollup1,
    [property: JsonPropertyName("confused")] long Confused,
    [property: JsonPropertyName("eyes")] long Eyes,
    [property: JsonPropertyName("heart")] long Heart,
    [property: JsonPropertyName("hooray")] long Hooray,
    [property: JsonPropertyName("laugh")] long Laugh,
    [property: JsonPropertyName("rocket")] long Rocket,
    [property: JsonPropertyName("total_count")] long TotalCount,
    [property: JsonPropertyName("url")] Uri Url);
