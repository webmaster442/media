// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interop;

internal sealed class FFMpegCommandBuilder : IBuilder<string>
{
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
        { CliSegment.VideoBitrate, "-b:v {0}" },
        { CliSegment.AudioSampleRate, "-ar {0}" },
        { CliSegment.StartTime, "-ss {0}" },
        { CliSegment.Duration, "-t {0}" },
        { CliSegment.AspectRatio, "-aspect {0}" },
        { CliSegment.VideoFilter, "-vf \"{0}\"" },
        { CliSegment.Vsync, "-vsync {0}" }
    };

    public FFMpegCommandBuilder()
    {
        _data = new Dictionary<CliSegment, string>();
    }

    private enum CliSegment
    {
        InputFile = int.MinValue,
        InputFile2 = int.MinValue + 1,
        InputFile3 = int.MinValue + 2,
        InputFile4 = int.MinValue + 3,
        InputFile5 = int.MinValue + 4,
        StartTime = 0,
        IgnoreVideo = 10,
        CompressionLevel = 20,
        AudioStreamSelect = 30,
        Duration = 35,
        AudioCodec = 40,
        AudioBitrate = 50,
        AudioFilter = 60,
        AudioSampleRate = 70,
        VideCodec = 80,
        VideoBitrate = 90,
        VideoQuality = 91,
        VideoFilter = 95,
        VideoAspect = 98,
        Target = 100,
        Vsync = 110,
        AspectRatio = 120,
        AdditionalsBeforeOutputFile = int.MaxValue - 1,
        OutputFile = int.MaxValue
    }

    public string Build()
    {
        var ordered = _data
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .OrderBy(x => x.Key)
            .Select(x => x.Value);

        return string.Join(" ", ordered);
    }

    public FFMpegCommandBuilder IgnoreVideo()
    {
        SetArgument(CliSegment.IgnoreVideo, string.Empty);
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

    public FFMpegCommandBuilder WithAdditionalsBeforeOutputFile(string cmd)
    {
        SetArgument(CliSegment.AdditionalsBeforeOutputFile, cmd);
        return this;
    }

    public FFMpegCommandBuilder WithAspectRatio(string aspectRatio)
    {
        SetArgument(CliSegment.AspectRatio, aspectRatio);
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

    public FFMpegCommandBuilder WithAudioFilter(string filterString)
    {
        SetArgument(CliSegment.AudioFilter, filterString);
        return this;
    }

    public FFMpegCommandBuilder WithAudioSampleRate(int sampleRate)
    {
        SetArgument(CliSegment.AudioSampleRate, sampleRate);
        return this;
    }

    public FFMpegCommandBuilder WithAudioStreamSelection(int streamIndex)
    {
        SetArgument(CliSegment.AudioStreamSelect, streamIndex);
        return this;
    }

    public FFMpegCommandBuilder WithCompressionLevel(int compressionLevel)
    {
        SetArgument(CliSegment.CompressionLevel, compressionLevel);
        return this;
    }

    public FFMpegCommandBuilder WithDurationInSeconds(double seconds)
    {
        SetArgument(CliSegment.Duration, seconds);
        return this;
    }

    public FFMpegCommandBuilder WithInputFile(string inputFile)
    {
        SetArgument(CliSegment.InputFile, inputFile);
        return this;
    }

    public FFMpegCommandBuilder WithOutputFile(string outputFile)
    {
        SetArgument(CliSegment.OutputFile, outputFile);
        return this;
    }

    public FFMpegCommandBuilder WithStartTimeInSeconds(double seconds)
    {
        SetArgument(CliSegment.StartTime, seconds);
        return this;
    }

    public FFMpegCommandBuilder WithVideoBitrate(string bitrate)
    {
        SetArgument(CliSegment.VideoBitrate, bitrate);
        return this;
    }

    public FFMpegCommandBuilder WithVideoQuality(int quality)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(quality, 2, nameof(quality));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(quality, 31, nameof(quality));
        SetArgument(CliSegment.VideoQuality, quality);
        return this;
    }

    public FFMpegCommandBuilder WithVideoCodec(string codecName)
    {
        SetArgument(CliSegment.VideCodec, codecName);
        return this;
    }

    public FFMpegCommandBuilder WithVideoFilter(string filterString)
    {
        SetArgument(CliSegment.VideoFilter, filterString);
        return this;
    }

    public FFMpegCommandBuilder WithVideoAspect(string aspectRatio)
    {
        SetArgument(CliSegment.VideoAspect, aspectRatio);
        return this;
    }

    public FFMpegCommandBuilder WithTarget(string target)
    {
        SetArgument(CliSegment.Target, target);
        return this;
    }

    public FFMpegCommandBuilder WithVsync(string vsync)
    {
        SetArgument(CliSegment.Vsync, vsync);
        return this;
    }

    private void SetArgument(CliSegment segment, object? value)
    {
        _data[segment] = value == null
            ? _segmentFormats[segment]
            : string.Format(CultureInfo.InvariantCulture, _segmentFormats[segment], value);
    }
}