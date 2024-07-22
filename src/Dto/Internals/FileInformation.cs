using System.Text.Json.Serialization;

namespace FFCmd.Dto.Internals;

internal class FileInformation
{
    [JsonPropertyOrder(0)]
    public required string FileName { get; init; }
    [JsonPropertyOrder(1)]
    public required string Format { get; init; }
    [JsonPropertyOrder(2)]
    public required string Duration { get; init; }
    [JsonPropertyOrder(3)]
    public required string Size { get; init; }
    [JsonPropertyOrder(4)]
    public required StreamInfo[] Streams { get; init; }
}
