using System.Text.Json.Serialization;

namespace FFCmd.Dto.FFProbe;

public record Tags([property: JsonPropertyName("ENCODER")] string Encoder);
