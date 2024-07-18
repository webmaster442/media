using System.Diagnostics;

namespace FFCmd.FFMpegInterop;

internal sealed class FFMpeg
{
    private static bool TryGetInstalledPath(out string ffmpegPath)
    {
        List<string> foldersToCheck = new()
        {
            AppDomain.CurrentDomain.BaseDirectory
        };
        foldersToCheck.AddRange(Environment.ExpandEnvironmentVariables("%path%").Split(';'));
        foreach (var folder in foldersToCheck)
        {
            ffmpegPath = Path.Combine(folder, "ffmpeg.exe");
            if (File.Exists(ffmpegPath))
            {
                return true;
            }
        }
        ffmpegPath = string.Empty;
        return false;
    }

    public static void StartFFMpeg(FFMpegCommandBuilder command)
    {
        if (!TryGetInstalledPath(out string ffmpegPath))
        {
            throw new InvalidOperationException("FFMpeg not found in the system.");
        }

        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = command.BuildCommandLine(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        process.Start();
    }
}
