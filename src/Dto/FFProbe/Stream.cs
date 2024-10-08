﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Media.Dto.FFProbe;

public record Stream(
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("codec_name")] string CodecName,
    [property: JsonPropertyName("codec_long_name")] string CodecLongName,
    [property: JsonPropertyName("profile")] string Profile,
    [property: JsonPropertyName("codec_type")] string CodecType,
    [property: JsonPropertyName("codec_tag_string")] string CodecTagString,
    [property: JsonPropertyName("codec_tag")] string CodecTag,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("coded_width")] int CodedWidth,
    [property: JsonPropertyName("coded_height")] int CodedHeight,
    [property: JsonPropertyName("closed_captions")] int ClosedCaptions,
    [property: JsonPropertyName("film_grain")] int FilmGrain,
    [property: JsonPropertyName("has_b_frames")] int HasBFrames,
    [property: JsonPropertyName("sample_aspect_ratio")] string SampleAspectRatio,
    [property: JsonPropertyName("display_aspect_ratio")] string DisplayAspectRatio,
    [property: JsonPropertyName("pix_fmt")] string PixFmt,
    [property: JsonPropertyName("level")] int Level,
    [property: JsonPropertyName("color_range")] string ColorRange,
    [property: JsonPropertyName("color_space")] string ColorSpace,
    [property: JsonPropertyName("chroma_location")] string ChromaLocation,
    [property: JsonPropertyName("field_order")] string FieldOrder,
    [property: JsonPropertyName("refs")] int Refs,
    [property: JsonPropertyName("is_avc")] string IsAvc,
    [property: JsonPropertyName("nal_length_size")] string NalLengthSize,
    [property: JsonPropertyName("r_frame_rate")] string RFrameRate,
    [property: JsonPropertyName("avg_frame_rate")] string AvgFrameRate,
    [property: JsonPropertyName("time_base")] string TimeBase,
    [property: JsonPropertyName("start_pts")] int StartPts,
    [property: JsonPropertyName("start_time")] string StartTime,
    [property: JsonPropertyName("bits_per_raw_sample")] string BitsPerRawSample,
    [property: JsonPropertyName("extradata_size")] int ExtradataSize,
    [property: JsonPropertyName("disposition")] Disposition Disposition,
    [property: JsonPropertyName("tags")] StreamTags Tags,
    [property: JsonPropertyName("sample_fmt")] string SampleFmt,
    [property: JsonPropertyName("sample_rate")] string SampleRate,
    [property: JsonPropertyName("channels")] int Channels,
    [property: JsonPropertyName("channel_layout")] string ChannelLayout,
    [property: JsonPropertyName("bits_per_sample")] int BitsPerSample,
    [property: JsonPropertyName("initial_padding")] int InitialPadding,
    [property: JsonPropertyName("bit_rate")] string BitRate,
    [property: JsonPropertyName("duration_ts")] int DurationTs,
    [property: JsonPropertyName("duration")] string Duration);
