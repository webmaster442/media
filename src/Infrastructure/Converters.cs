// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure;

internal static class Converters
{
    public static string SecondsToHumanTime(string seconds)
    {
        var human = TimeSpan.FromSeconds(double.Parse(seconds, CultureInfo.InvariantCulture));
        return human.ToString();
    }

    public static string BytesToHumanSize(string bytes)
    {
        long size = long.Parse(bytes, CultureInfo.InvariantCulture);
        string[] suffixes = { "B", "KiB", "MiB", "GiB", "TiB" };
        int suffixIndex = 0;

        while (size >= 1024 && suffixIndex < suffixes.Length - 1)
        {
            size /= 1024;
            suffixIndex++;
        }

        return $"{size} {suffixes[suffixIndex]}";
    }
}
