using System.Text.Json.Serialization;

namespace Media.Dto.FFProbe;

public record Tags([property: JsonPropertyName("ENCODER")] string Encoder);
