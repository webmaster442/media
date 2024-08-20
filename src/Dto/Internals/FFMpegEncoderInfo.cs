namespace Media.Dto.Internals;

public class FFMpegEncoderInfo
{
    public required string Name { get; init; }
    public required EncoderType Type  { get; init; }

    public enum EncoderType
    {
        Video,
        Audio,
        Subtitle,
    }
}
