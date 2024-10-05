using System.IO;

namespace Media.Tests.Autocomplete.Settings;

public class HorseSettings : MammalSettings
{
    [CommandOption("-d|--day")]
    public DayOfWeek Day { get; set; }

    [CommandOption("--file")]
    public FileInfo? File { get; set; }

    [CommandOption("--directory")]
    public DirectoryInfo? Directory { get; set; }
}