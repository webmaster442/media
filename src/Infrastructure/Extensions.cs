// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Internals;

namespace Media.Infrastructure;

public static class Extensions
{
    public static string? SkipToLine(this StringReader reader, string lineToSkipTo, int maxSkipCount = 100)
    {
        string? line;
        int count = 0;
        while ((line = reader.ReadLine()) != null)
        {
            if (line == lineToSkipTo)
            {
                return line;
            }
            ++count;
            if (count > maxSkipCount)
            {
                break;
            }
        }
        return null;
    }

    public static FFMpegEncoderInfo.EncoderType ToEncoderType(this char c)
    {
        return c switch
        {
            'V' => FFMpegEncoderInfo.EncoderType.Video,
            'A' => FFMpegEncoderInfo.EncoderType.Audio,
            'S' => FFMpegEncoderInfo.EncoderType.Subtitle,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, "Unknown encoder type"),
        };
    }
}
