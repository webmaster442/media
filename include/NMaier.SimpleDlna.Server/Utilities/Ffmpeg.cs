using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Server.Utilities;

using InfoCache = LeastRecentlyUsedDictionary<FileInfo, IDictionary<string, string>>;

public static class FFmpeg
{
    private static readonly InfoCache InfoCache = new InfoCache(500);

    private static readonly Regex RegAssStrip =
      new Regex(@"^,+", RegexOptions.Compiled);

    private static readonly Regex RegDuration = new Regex(
      @"Duration: ([0-9]{2}):([0-9]{2}):([0-9]{2})(?:\.([0-9]+))?",
      RegexOptions.Compiled);

    private static readonly Regex RegDimensions = new Regex(
      @"Video: .+ ([0-9]{2,})x([0-9]{2,}) ", RegexOptions.Compiled);

    public static readonly string? FFmpegExecutable = FindExecutable();

    private static DirectoryInfo GetFFMpegFolder(
      Environment.SpecialFolder folder)
    {
        return new DirectoryInfo(
          Path.Combine(Environment.GetFolderPath(folder), "ffmpeg"));
    }

    private static string? FindExecutable()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "ffmpeg.exe");
        if (File.Exists(path))
        {
            return path;
        }
        return null;
    }

    private static IDictionary<string, string> IdentifyFileInternal(
      FileInfo file)
    {
        if (FFmpegExecutable == null)
        {
            throw new NotSupportedException();
        }
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }
        IDictionary<string, string> rv;
        if (InfoCache.TryGetValue(file, out rv))
        {
            return rv;
        }
        try
        {
            return IdentifyInternalFromProcess(file);
        }
        catch (Exception ex)
        {
            throw new NotSupportedException(ex.Message, ex);
        }
    }

    private static IDictionary<string, string> IdentifyInternalFromProcess(
      FileInfo file)
    {
        using (var p = new Process())
        {
            var sti = p.StartInfo;
#if !DEBUG
    sti.CreateNoWindow = true;
#endif
            sti.UseShellExecute = false;
            sti.FileName = FFmpegExecutable;
            sti.Arguments = $"-i \"{file.FullName}\"";
            sti.RedirectStandardError = true;
            p.Start();
            IDictionary<string, string> rv = new Dictionary<string, string>();

            using (var reader = new StreamReader(StreamManager.GetStream()))
            {
                using (var pump = new StreamPump(
                  p.StandardError.BaseStream, reader.BaseStream, 4096))
                {
                    pump.Pump(null);
                    if (!p.WaitForExit(3000))
                    {
                        throw new NotSupportedException("ffmpeg timed out");
                    }
                    if (!pump.Wait(1000))
                    {
                        throw new NotSupportedException("ffmpeg pump timed out");
                    }
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    var output = reader.ReadToEnd();
                    var match = RegDuration.Match(output);
                    if (match.Success)
                    {
                        int h, m, s;
                        if (int.TryParse(match.Groups[1].Value, out h) &&
                            int.TryParse(match.Groups[2].Value, out m) &&
                            int.TryParse(match.Groups[3].Value, out s))
                        {
                            int ms;
                            if (match.Groups.Count < 5 ||
                                !int.TryParse(match.Groups[4].Value, out ms))
                            {
                                ms = 0;
                            }
                            var ts = new TimeSpan(0, h, m, s, ms * 10);
                            var tss = ts.TotalSeconds.ToString(
                              CultureInfo.InvariantCulture);
                            rv.Add("LENGTH", tss);
                        }
                    }
                    match = RegDimensions.Match(output);
                    if (match.Success)
                    {
                        int w, h;
                        if (int.TryParse(match.Groups[1].Value, out w) &&
                            int.TryParse(match.Groups[2].Value, out h))
                        {
                            rv.Add("VIDEO_WIDTH", w.ToString());
                            rv.Add("VIDEO_HEIGHT", h.ToString());
                        }
                    }
                }
            }
            if (rv.Count == 0)
            {
                throw new NotSupportedException("File not supported");
            }
            return rv;
        }
    }

    public static (int w, int h) GetFileDimensions(FileInfo file)
    {
        if (IdentifyFile(file).TryGetValue("VIDEO_WIDTH", out string? sw)
            && IdentifyFile(file).TryGetValue("VIDEO_HEIGHT", out string? sh)
            && int.TryParse(sw, out int w)
            && int.TryParse(sh, out int h)
            && w > 0 && h > 0)
        {
            return new(w, h);
        }
        throw new NotSupportedException();
    }

    public static double GetFileDuration(FileInfo file)
    {
        if (IdentifyFile(file).TryGetValue("LENGTH", out string? sl))
        {
            double dur;
            var valid = double.TryParse(
              sl, NumberStyles.AllowDecimalPoint,
              CultureInfo.GetCultureInfo("en-US", "en"), out dur);
            if (valid && dur > 0)
            {
                return dur;
            }
        }
        throw new NotSupportedException();
    }

    public static string GetSubtitleSubrip(FileInfo file)
    {
        if (FFmpegExecutable == null)
        {
            throw new NotSupportedException();
        }
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }
        try
        {
            using (var p = new Process())
            {
                var sti = p.StartInfo;
#if !DEBUG
      sti.CreateNoWindow = true;
#endif
                sti.UseShellExecute = false;
                sti.FileName = FFmpegExecutable;
                sti.Arguments = $"-i \"{file.FullName}\" -map s:0 -f srt pipe:";
                sti.RedirectStandardOutput = true;
                p.Start();

                var lastPosition = 0L;
                using (var reader = new StreamReader(StreamManager.GetStream()))
                {
                    using (var pump = new StreamPump(
                      p.StandardOutput.BaseStream, reader.BaseStream, 100))
                    {
                        pump.Pump(null);
                        while (!p.WaitForExit(20000))
                        {
                            if (lastPosition != reader.BaseStream.Position)
                            {
                                lastPosition = reader.BaseStream.Position;
                                continue;
                            }
                            p.Kill();
                            throw new NotSupportedException("ffmpeg timed out");
                        }
                        if (!pump.Wait(2000))
                        {
                            throw new NotSupportedException("ffmpeg pump timed out");
                        }
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);

                        var rv = string.Empty;
                        string? line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            rv += RegAssStrip.Replace(line.Trim(), string.Empty) + "\n";
                        }
                        if (!string.IsNullOrWhiteSpace(rv))
                        {
                            return rv;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new NotSupportedException(ex.Message, ex);
        }
        throw new NotSupportedException(
          "File does not contain a valid subtitle");
    }

    public static IDictionary<string, string> IdentifyFile(FileInfo file)
    {
        if (FFmpegExecutable != null)
        {
            return IdentifyFileInternal(file);
        }
        throw new NotSupportedException();
    }
}