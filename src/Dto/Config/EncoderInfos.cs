// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Internals;

namespace Media.Dto.Config;

public sealed class EncoderInfos
{
    public required DateTimeOffset Version { get; init; }
    public required FFMpegEncoderInfo[] Encoders { get; init; }
}