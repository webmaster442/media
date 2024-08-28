using System.Diagnostics;

using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.Server.Types;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Thumbnails;

internal sealed class VideoThumbnailLoader
: Logging, IThumbnailLoader, IDisposable
{
    private Semaphore? semaphore = new Semaphore(5, 5);

    public VideoThumbnailLoader()
    {
        if (FFmpeg.FFmpegExecutable == null)
        {
            throw new NotSupportedException("No ffmpeg available");
        }
    }

    public void Dispose()
    {
        if (semaphore != null)
        {
            semaphore.Dispose();
            semaphore = null;
        }
    }

    public DlnaMediaTypes Handling => DlnaMediaTypes.Video;

    public MemoryStream GetThumbnail(object item, ref int width,
      ref int height)
    {
        semaphore?.WaitOne();
        try
        {
            var stream = item as Stream;
            if (stream != null)
            {
                return GetThumbnailInternal(stream, ref width, ref height);
            }
            var fi = item as FileInfo;
            if (fi != null)
            {
                return GetThumbnailInternal(fi, ref width, ref height);
            }
            throw new NotSupportedException();
        }
        finally
        {
            semaphore?.Release();
        }
    }

    private static MemoryStream GetThumbnailFromProcess(Process p,
      ref int width,
      ref int height)
    {
        var lastPosition = 0L;
        using (var thumb = StreamManager.GetStream())
        {
            using (var pump = new StreamPump(
              p.StandardOutput.BaseStream, thumb, 4096))
            {
                pump.Pump(null);
                while (!p.WaitForExit(20000))
                {
                    if (lastPosition != thumb.Position)
                    {
                        lastPosition = thumb.Position;
                        continue;
                    }
                    p.Kill();
                    throw new ArgumentException("ffmpeg timed out");
                }
                if (p.ExitCode != 0)
                {
                    throw new ArgumentException("ffmpeg does not understand the stream");
                }
                if (!pump.Wait(2000))
                {
                    throw new ArgumentException("stream reading timed out");
                }
                if (thumb.Length == 0)
                {
                    throw new ArgumentException("ffmpeg did not produce a result");
                }

                return thumb;
            }
        }
    }

    private static MemoryStream GetThumbnailInternal(Stream stream,
      ref int width,
      ref int height)
    {
        using (var p = new Process())
        {
            var pos = 20L;
            try
            {
                var length = stream.Length;
                if (length < 10 * (1 << 20))
                {
                    pos = 5;
                }
                else
                {
                    if (length > 100 * (1 << 20))
                    {
                        pos = 60;
                    }
                    else
                    {
                        if (length > 50 * (1 << 20))
                        {
                            pos = 60;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            var sti = p.StartInfo;
#if !DEBUG
    sti.CreateNoWindow = true;
#endif
            sti.UseShellExecute = false;
            sti.FileName = FFmpeg.FFmpegExecutable;
            sti.Arguments = $"-v quiet -ss {pos} -i pipe: -an -frames:v 1 -f image2  pipe:";
            sti.RedirectStandardInput = true;
            sti.RedirectStandardOutput = true;
            p.Start();

            using var sp = new StreamPump(stream, p.StandardInput.BaseStream, 4096);
            sp.Pump((pump, result) => stream.Dispose());
            return GetThumbnailFromProcess(p, ref width, ref height);
        }
    }

    private MemoryStream GetThumbnailInternal(FileInfo file, ref int width,
      ref int height)
    {
        Exception last = null;
        for (var best = IdentifyBestCapturePosition(file);
          best >= 0;
          best -= Math.Max(best / 2, 5))
        {
            try
            {
                using (var p = new Process())
                {
                    var sti = p.StartInfo;
#if !DEBUG
        sti.CreateNoWindow = true;
#endif
                    sti.UseShellExecute = false;
                    sti.FileName = FFmpeg.FFmpegExecutable;
                    sti.Arguments = $"-v quiet -ss {best} -i \"{file.FullName}\" -an -frames:v 1 -f image2 pipe:";
                    sti.RedirectStandardOutput = true;
                    p.Start();

                    DebugFormat("Running: {0} {1}", sti.FileName, sti.Arguments);
                    return GetThumbnailFromProcess(p, ref width, ref height);
                }
            }
            catch (Exception ex)
            {
                last = ex;
            }
        }
        throw last ?? new Exception("Not reached");
    }

    private long IdentifyBestCapturePosition(FileInfo file)
    {
        try
        {
            var dur = FFmpeg.GetFileDuration(file);
            if (dur > 600)
            {
                return (long)(dur / 5.0);
            }
            return (long)(dur / 3.0);
        }
        catch (Exception ex)
        {
            Debug("Failed to get file duration", ex);
        }
        var length = file.Length;
        if (length < 10 * (1 << 20))
        {
            return 5;
        }
        if (length > 50 * (1 << 20))
        {
            return 60;
        }
        if (length > 100 * (1 << 20))
        {
            return 120;
        }
        if (length > 500 * (1 << 20))
        {
            return 300;
        }
        if (length > 750 * (1 << 20))
        {
            return 600;
        }
        return 20;
    }
}
