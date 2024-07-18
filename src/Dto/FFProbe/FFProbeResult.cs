using System.Text.Json.Serialization;

namespace FFCmd.Dto.FFProbe;

public record FFProbeResult(
    [property: JsonPropertyName("streams")] Stream[] Streams,
    [property: JsonPropertyName("format")] Format Format);
