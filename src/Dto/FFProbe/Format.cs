// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Media.Dto.FFProbe;

public record Format([property: JsonPropertyName("filename")] string Filename,
                     [property: JsonPropertyName("nb_streams")] int NbStreams,
                     [property: JsonPropertyName("nb_programs")] int NbPrograms,
                     [property: JsonPropertyName("nb_stream_groups")] int NbStreamGroups,
                     [property: JsonPropertyName("format_name")] string formatName,
                     [property: JsonPropertyName("format_long_name")] string FormatLongName,
                     [property: JsonPropertyName("start_time")] string StartTime,
                     [property: JsonPropertyName("duration")] string Duration,
                     [property: JsonPropertyName("size")] string Size,
                     [property: JsonPropertyName("bit_rate")] string BitRate,
                     [property: JsonPropertyName("probe_score")] int ProbeScore,
                     [property: JsonPropertyName("tags")] Tags Tags);
