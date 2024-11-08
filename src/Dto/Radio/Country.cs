using System.Text.Json.Serialization;

namespace Media.Dto.Radio;

public record Country(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("iso_3166_1")] string Iso31661,
    [property: JsonPropertyName("stationcount")] int Stationcount);
