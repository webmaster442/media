// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto.Internals;

internal sealed record class YtDlpFormat
{
    public required string Id { get; init; }
    public required string Format { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required int BitrateInK { get; init; }
    public required string Codec { get; init; }
}
