// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Dlna;
using Media.Dto.Internals;

namespace Media.Infrastructure;

internal static class Parsers
{
    public static SSDP ParseSSDPResponse(string ssdpResponse)
    {
        SSDP result = new();
        string[] lines = ssdpResponse.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            int colonIndex = line.IndexOf(':');
            if (colonIndex > 0)
            {
                string key = line[..colonIndex];
                result[key] = line[(colonIndex + 1)..];
            }
        }
        return result;
    }

    public static IEnumerable<YtDlpFormat> ParseFormats(string formatText)
    {
        static (string id, string format, int width, int height) ParseColumn0(string column0)
        {
            var idFormatSize = column0.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!idFormatSize[2].Contains('x'))
            {
                return (idFormatSize[0], idFormatSize[1], 0, 0);
            }
            var size = idFormatSize[2].Split('x', StringSplitOptions.RemoveEmptyEntries);
            return (idFormatSize[0], idFormatSize[1], int.Parse(size[0]), int.Parse(size[1]));
        }

        static string ExtractSubColumn(string column, Func<int, int> columnSelector, string defaultValue = "")
        {
            var subColumns = column.Split([' ', '~'], StringSplitOptions.RemoveEmptyEntries);
            int selected = columnSelector(subColumns.Length);
            if (selected < subColumns.Length
                && selected > -1)
            {
                return subColumns[selected];
            }
            return defaultValue;
        }

        using var reader = new StringReader(formatText);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)
                || !line.Contains('|')
                || line.StartsWith("ID"))
            {
                continue;
            }

            var columns = line.Split('|', StringSplitOptions.RemoveEmptyEntries);

            var (id, format, width, height) = ParseColumn0(columns[0]);

            yield return new YtDlpFormat
            {
                Id = id,
                Format = format,
                Width = width,
                Height = height,
                BitrateInK = int.Parse(ExtractSubColumn(columns[1], len => len == 3 ? 1 : 2, "0").Replace("k", "")),
                Codec = ExtractSubColumn(columns[2], _ => 0),
            };
        }
    }

    public static IEnumerable<FFMpegEncoderInfo> ParseEncoderInfos(string encoderInfos)
    {
        using var reader = new StringReader(encoderInfos);
        string? line;

        reader.SkipToLine("------");

        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            yield return new FFMpegEncoderInfo
            {
                Name = parts[1],
                Description = string.Join(' ', parts[2..]),
                Type = parts[0][0].ToEncoderType(),
            };
        }
    }
}
