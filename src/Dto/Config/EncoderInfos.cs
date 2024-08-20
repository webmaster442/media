using Media.Dto.Internals;

namespace Media.Dto.Config;

public sealed class EncoderInfos
{
    public required DateTimeOffset Version { get; init; }
    public required FFMpegEncoderInfo[] Encoders { get; init; }
}