using System.Text.Json.Serialization;

namespace Media.Dto.FFProbe;

public record FFProbeResult(
    [property: JsonPropertyName("streams")] Stream[] Streams,
    [property: JsonPropertyName("format")] Format Format);
