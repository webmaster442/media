// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.Validation;
using Media.Interop.CdRip;

using Spectre.Console;

namespace Media.Commands;
internal sealed class CdRip : AsyncCommand<CdRip.Settings>
{
    public class Settings : ValidatedCommandSettings
    {
        [Required]
        [DriveLetter]
        [CommandArgument(0, "<cd drive letter>")]
        [Description("The drive letter of the cd drive to rip")]
        public string DriveLetter { get; set; } = string.Empty;

        [Required]
        [DirectoryExists]
        [CommandArgument(1, "<target directory>")]
        [Description("The directory where the ripped files will be saved")]
        public string TargetDirectory { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
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

            var toc = await drive.ReadTableOfContents();
            if (toc == null)
            {

                Terminal.RedText("Failed to read table of contents");
                return ExitCodes.Error;
            }

            var reader = new TrackReader(drive);

            using var tokenSource = new ConsoleCancelTokenSource();

            await AnsiConsole.Progress().AutoRefresh(false).StartAsync(async ctx =>
            {
                foreach (var track in toc.Tracks)
                {
                    tokenSource.Token.ThrowIfCancellationRequested();

                    var fileName = Path.Combine(settings.TargetDirectory, $"Track-{track.TrackNumber}.wav");
                    var task = ctx.AddTask($"Ripping track {track.TrackNumber} to {fileName}");

                    using (var wav = new WaveFileWriter(File.Create(fileName), false))
                    {
                        uint trackSize = (uint)track.Sectors * Constants.CB_AUDIO;
                        wav.WriteHeader(44100, 16, 2, trackSize);
                        await reader.ReadTrackAsync(track, data => wav.WriteData(data), (long position, long length) =>
                        {
                            task.Value = (double)position / length * 100;
                            ctx.Refresh();
                        }, tokenSource.Token);
                    }

                }
            });
            await drive.UnLockAsync();
            return ExitCodes.Success;
        }
        catch (Exception ex)
        {
            await drive.UnLockAsync();
            Terminal.DisplayException(ex);
            return ExitCodes.Exception;
        }
    }
}
