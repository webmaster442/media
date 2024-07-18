using System.Diagnostics;
using System.Text.Json;

using FFCmd.Dto.FFProbe;

using Spectre.Console;

namespace FFCmd.FFMpegInterop;

internal sealed class FFMpeg
{
    private const string ffmpegBinary = "ffmpeg.exe";
    private const string ffprobeBinary = "ffprobe.exe";
    private const string ffmpegVersionFile = "ffmpeg.json";

    private static bool TryGetInstalledPath(out string ffmpegPath)
    {
        ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ffmpegBinary);
        return File.Exists(ffmpegPath);
    }

    private static bool TryGetFFProbePath(out string ffprobePath)
    {
        ffprobePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ffprobeBinary);
        return File.Exists(ffprobePath);
    }

    public static async Task SetInstalledVersion(DateTimeOffset? publishedAt)
    {
        var versionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ffmpegVersionFile);
        using var stream = File.Create(versionFile);
        await JsonSerializer.SerializeAsync(stream, publishedAt);
    }

    public static async Task<DateTimeOffset?> GetInstalledVersion()
    {
        var versionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ffmpegVersionFile);
        if (!File.Exists(versionFile))
        {
            return null;
        }
        using var stream = File.OpenRead(versionFile);
        return await JsonSerializer.DeserializeAsync<DateTimeOffset>(stream);
    }

    public async static Task<FFProbeResult> FFProbe(string file)
    {
        if (!TryGetInstalledPath(out string ffprobePath))
        {
            throw new InvalidOperationException("FFProbe not found.");
        }

        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffprobePath,
                Arguments = $"-v quiet -print_format json -show_format -show_streams \"{file}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            }
        };
        process.Start();
        var result = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        return JsonSerializer.Deserialize<FFProbeResult>(result ?? string.Empty)
            ?? throw new InvalidOperationException("FFProbe result can't be parsed");
    }

    public static void StartFFMpeg(FFMpegCommandBuilder command)
    {
        if (!TryGetInstalledPath(out string ffmpegPath))
        {
            throw new InvalidOperationException("FFMpeg not found.");
        }

        var commandLine = command.BuildCommandLine();

        AnsiConsole.MarkupLineInterpolated($"[green]Executing: [/]{commandLine.EscapeMarkup()}");

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
