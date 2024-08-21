// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Media.Dto.Internals;

public sealed record class FFMpegEncoderInfo
{
    [JsonPropertyName("n")]
    public required string Name { get; init; }
    [JsonPropertyName("d")]
    public required string Description { get; init; }
    [JsonPropertyName("t")]
    public required EncoderType Type { get; init; }

    public enum EncoderType
    {
        Video,
        Audio,
        Subtitle,
    }
}
