
namespace FFCmd.FFMpegInterop;

internal sealed class FFMpegCommandBuilder
{
    private enum CliSegment
    {
        InputFile = int.MinValue,
        InputFile2 = int.MinValue + 1,
        InputFile3 = int.MinValue + 2,
        InputFile4 = int.MinValue + 3,
        InputFile5 = int.MinValue + 4,
        IgnoreVideo = 0,
        CompressionLevel = 1,
        AudioStreamSelect = 2,
        AudioCodec = 3,
        AudioBitrate = 4,
        AudioFilter = 5,
        VideCodec = 6,
        AdditionalsBeforeOutputFile = int.MaxValue - 1,
        OutputFile = int.MaxValue
    }

    private readonly Dictionary<CliSegment, string> _data;

    private readonly Dictionary<CliSegment, string> _segmentFormats = new()
    {
        { CliSegment.InputFile, "-i \"{0}\"" },
        { CliSegment.InputFile2, "-i \"{0}\"" },
        { CliSegment.InputFile3, "-i \"{0}\"" },
        { CliSegment.InputFile4, "-i \"{0}\"" },
        { CliSegment.OutputFile, "\"{0}\"" },
        { CliSegment.IgnoreVideo, "-vn" },
        { CliSegment.CompressionLevel, "-compression_level {0}" },
        { CliSegment.AudioBitrate, "-b:a {0}" },
        { CliSegment.AudioCodec, "-c:a {0}" },
        { CliSegment.AudioFilter, "-af \"{0}\"" },
        { CliSegment.AudioStreamSelect, "-map 0:a:{0}" },
        { CliSegment.AdditionalsBeforeOutputFile, "{0}" },
        { CliSegment.VideCodec, "-c:v {0}" },
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

    public FFMpegCommandBuilder WithAdditionalInputFiles(params string[] inputFile)
    {
        if (inputFile.Length > 4)
            throw new InvalidOperationException("Only 4 additional inputs can be applied");

        CliSegment[] inputSegments = [CliSegment.InputFile2, CliSegment.InputFile3, CliSegment.InputFile4, CliSegment.InputFile5];
        for (int i = 0; i < inputFile.Length; i++)
        {
            SetArgument(inputSegments[i], inputFile[i]);
        }
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

    public FFMpegCommandBuilder WithVideoCodec(string codecName)
    {
        SetArgument(CliSegment.VideCodec, codecName);
        return this;
    }

    public FFMpegCommandBuilder WithAudioFilter(string filterString)
    {
        SetArgument(CliSegment.AudioFilter, filterString);
        return this;
    }

    public FFMpegCommandBuilder WithCompressionLevel(int compressionLevel)
    {
        SetArgument(CliSegment.CompressionLevel, compressionLevel);
        return this;
    }

    public FFMpegCommandBuilder WithAdditionalsBeforeOutputFile(string cmd)
    {
        SetArgument(CliSegment.AdditionalsBeforeOutputFile, cmd);
        return this;
    }

    public FFMpegCommandBuilder WithOutputFile(string outputFile)
    {
        SetArgument(CliSegment.OutputFile, outputFile);
        return this;
    }

    public FFMpegCommandBuilder WithAudioStreamSelection(int streamIndex)
    {
        SetArgument(CliSegment.AudioStreamSelect, streamIndex);
        return this;
    }

    public string BuildCommandLine()
    {
        var ordered = _data.OrderBy(x => x.Key).Select(x => x.Value);
        return string.Join(" ", ordered);
    }
}
