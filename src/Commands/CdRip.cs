using Media.Infrastructure;
using Media.Infrastructure.Validation;
using Media.Interop;
using Media.Interop.CdRip;

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

            FFMpegCommandBuilder builder = new FFMpegCommandBuilder();

            var reader = new TrackReader(drive);

            foreach (var track in toc.Tracks)
            {
                var fileName = Path.Combine(settings.TargetDirectory, $"Track-{track.TrackNumber}.flac");
                Terminal.InfoText($"Ripping track {track.TrackNumber} to {fileName}");

            }
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
