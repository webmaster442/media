// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Internals;
using Media.Infrastructure;

namespace Media.Commands;

internal sealed class InfoDrives : Command
{
    private static string Get(DriveInfo drive, Func<DriveInfo, string> selector)
    {
        try
        {
            return selector.Invoke(drive);
        }
        catch (IOException)
        {
            return "N/A";
        }
    }

    public override int Execute(CommandContext context)
    {

        List<DriveData> drives = new();
        foreach (var drive in DriveInfo.GetDrives())
        {
            drives.Add(new DriveData
            {
                Name = drive.Name,
                Label = Get(drive, d => d.VolumeLabel),
                Format = Get(drive, d => d.DriveFormat),
                Type = drive.DriveType.ToString(),
                TotalHumanSize = Get(drive, d => d.TotalSize.ToHumanReadableSize()),
                AvailableHumanSize = Get(drive, d => d.AvailableFreeSpace.ToHumanReadableSize()),
                PercentUsed = Get(drive, drive =>
                {
                    double realPercent = 1.0 - ((double)drive.AvailableFreeSpace / (double)drive.TotalSize);
                    int percent = (int)(realPercent * 100);
                    return percent.ToString();
                })
            });
        }

        Terminal.DisplayTable(drives);

        return ExitCodes.Success;
    }
}
