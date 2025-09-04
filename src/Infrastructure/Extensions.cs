// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Text.RegularExpressions;

using Media.Dto;
using Media.Dto.Internals;
using Media.Dto.Organizer;

namespace Media.Infrastructure;

public static class Extensions
{
    public static (DateTime start, DateTime end) Day(this DateTime dateTime)
    {
        return (dateTime.Date, dateTime.Date.AddDays(1).AddTicks(-1));
    }

    public static (DateTime start, DateTime end) Last3Days(this DateTime dateTime)
    {
        return (dateTime.Date.AddDays(-3).Date, dateTime.Date.AddDays(1).AddTicks(-1));
    }

    public static (DateTime start, DateTime end) Week(this DateTime dateTime)
    {
        var diff = dateTime.DayOfWeek - CultureInfo.CurrentUICulture.DateTimeFormat.FirstDayOfWeek;
        if (diff < 0)
        {
            diff += 7;
        }
        var startOfWeek = dateTime.AddDays(-1 * diff).Date;
        var endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);
        return (startOfWeek, endOfWeek);
    }

    public static (DateTime start, DateTime end) Month(this DateTime dateTime)
    {
        var startOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
        return (startOfMonth, endOfMonth);
    }


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

    public static void AddArguments(this ProcessStartInfo startInfo, IEnumerable<string> items)
    {
        foreach (var item in items)
        {
            startInfo.ArgumentList.Add(item);
        }
    }

    public static void AddRange<T>(this BindingList<T> list, IEnumerable<T> items)
    {
        list.RaiseListChangedEvents = false;
        foreach (var item in items)
        {
            list.Add(item);
        }
        list.RaiseListChangedEvents = true;
        list.ResetBindings();
    }

    public static string ToHumanReadableSize(this long size)
    {
        string[] sizes = ["B", "KiB", "MiB", "GiB", "TiB", "PiB"];
        int order = 0;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    public static IEnumerable<string> GetMathcingFiles(this Rule rule, string rootFolder)
    {
        static IEnumerable<string> Filter(string[] files, string pattern, bool ignoreCase)
        {
            RegexOptions options = RegexOptions.Compiled;
            if (ignoreCase)
            {
                options |= RegexOptions.IgnoreCase;
            }
            var regex = new Regex(pattern, options);
            foreach (var file in files)
            {
                if (regex.IsMatch(file))
                {
                    yield return file;
                }
            }
        }

        static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
            Replace("\\*", ".*").
            Replace("\\?", ".") + "$";
        }

        var files = Directory.GetFiles(rootFolder, "*.*", SearchOption.TopDirectoryOnly);

        foreach (var pattern in rule.Patterns)
        {
            string filterRegex = pattern.Value;
            if (!pattern.IsRegex)
            {
                filterRegex = WildcardToRegex(pattern.Value);
            }
            foreach (var file in Filter(files, filterRegex, pattern.IgnoreCase))
            {
                yield return file;
            }
        }
    }
}