using System.Text.Json.Serialization;

namespace Media.Dto.Internals;

public class StreamInfo
{
    [JsonPropertyOrder(0)]
    public int Index { get; init; }
    [JsonPropertyOrder(1)]
    public required string Type { get; init; }
    [JsonPropertyOrder(2)]
    public required string Codec { get; init; }
    [JsonPropertyOrder(3)]
    public string? Width { get; init; }
    [JsonPropertyOrder(4)]
    public string? Height { get; init; }
    [JsonPropertyOrder(5)]
    public string? Channels { get; init; }
    [JsonPropertyOrder(6)]
    public string? SampleRate { get; init; }
    [JsonPropertyOrder(7)]
    public string? FrameRate { get; init; }
}
