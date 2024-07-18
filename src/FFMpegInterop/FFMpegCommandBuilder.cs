
namespace FFCmd.FFMpegInterop;

internal sealed class FFMpegCommandBuilder
{
    private enum CliSegment
    {
        InputFile = int.MinValue,
        IgnoreVideo = 0,
        CompressionLevel = 1,
        AudioCodec = 2,
        AudioBitrate = 3,
        OutputFile = int.MaxValue
    }

    private readonly Dictionary<CliSegment, string> _data;

    private readonly Dictionary<CliSegment, string> _segmentFormats = new()
    {
        { CliSegment.InputFile, "-i \"{0}\"" },
        { CliSegment.OutputFile, "\"{0}\"" },
        { CliSegment.IgnoreVideo, "-vn" },
        { CliSegment.CompressionLevel, "-compression_level {0}" },
        { CliSegment.AudioBitrate, "-b:a {0}" },
        { CliSegment.AudioCodec, "-c:a {0}" },
    };

    private void SetArgument(CliSegment segment, object? value)
    {
        _data[segment] = value == null
            ? _segmentFormats[segment]
            : string.Format(_segmentFormats[segment], value);
    }

    public FFMpegCommandBuilder()
    {
        _data = new Dictionary<CliSegment, string>();
    }

    public FFMpegCommandBuilder WithInputFile(string inputFile)
    {
        SetArgument(CliSegment.InputFile, inputFile);
        return this;
    }

    public FFMpegCommandBuilder IgnoreVideo()
    {
        SetArgument(CliSegment.IgnoreVideo, string.Empty);
        return this;
    }

    public FFMpegCommandBuilder WithAudioBitrate(string bitrate)
    {
        SetArgument(CliSegment.AudioBitrate, bitrate);
        return this;
    }

    public FFMpegCommandBuilder WithAudioCodec(string codecName)
    {
        SetArgument(CliSegment.AudioCodec, codecName);
        return this;
    }

    public FFMpegCommandBuilder WithCompressionLevel(int compressionLevel)
    {
        SetArgument(CliSegment.CompressionLevel, compressionLevel);
        return this;
    }

    public FFMpegCommandBuilder WithOutputFile(string outputFile)
    {
        SetArgument(CliSegment.OutputFile, outputFile);
        return this;
    }

    public string BuildCommandLine()
    {
        var ordered = _data.OrderBy(x => x.Key).Select(x => x.Value);
        return string.Join(" ", ordered);
    }
}
