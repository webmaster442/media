// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Media.Dto.FFProbe;

public record FFProbeResult(
    [property: JsonPropertyName("streams")] Stream[] Streams,
    [property: JsonPropertyName("format")] Format Format);
