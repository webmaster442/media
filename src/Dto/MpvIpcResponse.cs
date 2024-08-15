using System.Text.Json.Serialization;

namespace Media.Dto;
internal class MpvIpcResponse
{
    [JsonPropertyName("error")]
    public required string Error { get; init; }
    public string? Data { get; init; }

    [JsonIgnore]
    public bool IsSuccess => Error == "success";
}
