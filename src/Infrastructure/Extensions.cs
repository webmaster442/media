// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;

using Media.Dto;
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

    public static string GetCommandLine(this Preset preset, string inputFile, string outputFile)
    {
        return preset.CommandLine.Trim()
            .Replace(Preset.InputPlaceHolder, $"\"{inputFile}\"")
            .Replace(Preset.OutputPlaceHolder, $"\"{outputFile}\"");
    }

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    public static string ToHumanReadableSize(this long size)
    {
        string[] sizes = { "B", "KiB", "MiB", "GiB", "TiB", "PiB" };
        int order = 0;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size = size / 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    public static bool IsYoungerThan(this DateTime cacheTime, TimeSpan maxAge, DateTime? now = null)
    {
        if (now == null)
        {
            now = DateTime.UtcNow;
        }

        return (now.Value - cacheTime) < maxAge;
    }
}