// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Interop.CdRip;

using Spectre.Console;

namespace Media.Commands;

[Example("List cd tracks", @"media cdlist D:\")]
internal sealed class CdList : AsyncCommand<BaseCdSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, BaseCdSettings settings)
    {
        using var drive = CdDrive.Create(settings.DriveLetter);
        try
        {
            var discInDrive = await drive.IsCdInDriveAsync();
            if (!discInDrive)
            {
                Terminal.RedText("No disc in drive");
                return ExitCodes.Error;
            }

            await drive.LockAsync();

            var toc = await drive.ReadTableOfContentsAsync();

            if (toc == null)
            {
                await drive.UnLockAsync();
                Terminal.RedText("Failed to read table of contents");
                return ExitCodes.Error;
            }

            var table = new Table();
            table.AddColumn("Track").AddColumn("Length").AddColumn("Size");

            long sum = 0;
            TimeSpan total = TimeSpan.FromSeconds(0);
            foreach (var track in toc.Tracks)
            {
                long fileSize = track.Sectors * Constants.CB_AUDIO;
                sum += fileSize;
                total += track.Length;
                table.AddRow(track.TrackNumber.ToString(), track.Length.ToString(), Converters.BytesToHumanSize(fileSize));
            }
            AnsiConsole.Write(table);
            Terminal.InfoText($"Total size: {Converters.BytesToHumanSize(sum)}");
            Terminal.InfoText($"Total length: {total}");

            await drive.UnLockAsync();
            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            await drive.UnLockAsync();
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }
}
