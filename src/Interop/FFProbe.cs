using System.Diagnostics;

using Media.Dto.FFProbe;
using Media.Dto.Internals;
using Media.Infrastructure;

namespace Media.Interop;
internal static class FFProbe
{
    private const string FfprobeBinary = "ffprobe.exe";

    private static bool TryGetFFProbePath(out string ffprobePath)
    {
        ffprobePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FfprobeBinary);
        return File.Exists(ffprobePath);
    }

    public async static Task<FFProbeResult> GetFFProbeResult(string file)
    {
        if (!TryGetFFProbePath(out string ffprobePath))
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

    public static FileInformation Transform(FFProbeResult result)
    {
        var info = new FileInformation
        {
            Duration = Converters.SecondsToHumanTime(result.Format.Duration),
            FileName = result.Format.Filename,
            Format = result.Format.FormatLongName,
            Size = Converters.BytesToHumanSize(result.Format.Size),
            Streams = new StreamInfo[result.Streams.Length],
        };
        for (int i = 0; i < result.Streams.Length; i++)
        {
            info.Streams[i] = new StreamInfo
            {
                Index = i,
                Codec = result.Streams[i].CodecLongName,
                Type = result.Streams[i].CodecType,
                Channels = result.Streams[i].Channels > 0 ? result.Streams[i].Channels.ToString() : null,
                FrameRate = result.Streams[i].AvgFrameRate != "0/0" ? result.Streams[i].AvgFrameRate : null,
                SampleRate = result.Streams[i].SampleRate != "0" ? result.Streams[i].SampleRate : null,
                Height = GetSize(result.Streams[i].Height, result.Streams[i].CodedHeight),
                Width = GetSize(result.Streams[i].Width, result.Streams[i].CodedWidth),
            };
        }

        return info;
    }

    private static string? GetSize(int size, int coded)
    {
        if (size == 0)
            return null;

        if (size != coded)
        {
            return $"{size} (coded: {coded})";
        }
        return size.ToString();
    }
}
