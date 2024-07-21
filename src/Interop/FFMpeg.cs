using System.Diagnostics;

namespace FFCmd.Interop;

internal sealed class FFMpeg
{
    private const string ffmpegBinary = "ffmpeg.exe";

    private static bool TryGetInstalledPath(out string ffmpegPath)
    {
        ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ffmpegBinary);
        return File.Exists(ffmpegPath);
    }

    public static void StartFFMpeg(string commandLine)
    {
        if (!TryGetInstalledPath(out string ffmpegPath))
        {
            throw new InvalidOperationException("FFMpeg not found.");
        }

        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = commandLine,
                UseShellExecute = false,
            }
        };

        process.Start();
    }
}
