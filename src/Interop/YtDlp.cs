using System.Diagnostics;

using FFCmd.Dto.Internals;

namespace FFCmd.Interop;

internal static class YtDlp
{
    public static string CreateDownloadArguments(IEnumerable<YtDlpFormat> formats, YtDlpQuality qualityToSelect, string videoUrl)
    {
        static (int minHeight, int maxHeight) QualityToConstraints(YtDlpQuality quality)
        {
            return quality switch
            {
                YtDlpQuality.SdMp4 => ((int)(480 * 0.97), (int)(480 * 1.03)),
                YtDlpQuality.Hd720Mp4 => ((int)(720 * 0.97), (int)(720 * 1.03)),
                YtDlpQuality.Hd1080Mp4 => ((int)(1080 * 0.97), (int)(1080 * 1.03)),
                YtDlpQuality.Hd1440Mp4 => ((int)(1440 * 0.97), (int)(1440 * 1.03)),
                YtDlpQuality.Hd2160Mp4 => ((int)(2160 * 0.97), (int)(2160 * 1.03)),
                YtDlpQuality.AudioM4a => (0, 0),
                YtDlpQuality.AudioWebm => (0, 0),
                _ => throw new UnreachableException(),
            };
        }

        static YtDlpFormat SelectVideo(IEnumerable<YtDlpFormat> formats, YtDlpQuality qualityToSelect)
        {
            var (minHeight, maxHeight) = QualityToConstraints(qualityToSelect);

            return formats
                .Where(f => f.Height > minHeight
                    && f.Height < maxHeight
                    && f.Format == "mp4")
                .OrderByDescending(f => f.BitrateInK)
                .First();
        }

        static YtDlpFormat SelectBestM4AAudio(IEnumerable<YtDlpFormat> formats)
        {
            return formats
                .Where(f => f.Format == "m4a")
                .OrderByDescending(f => f.BitrateInK)
                .First();
        }

        static YtDlpFormat SelectBestWebmAAudio(IEnumerable<YtDlpFormat> formats)
        {
            return formats
                .Where(f => f.Format == "webm")
                .OrderByDescending(f => f.BitrateInK)
                .First();
        }

        if (qualityToSelect == YtDlpQuality.AudioM4a)
        {
            var m4a = SelectBestM4AAudio(formats);
            return $"-f {m4a.Id} {videoUrl}";
        }
        else if (qualityToSelect == YtDlpQuality.AudioWebm)
        {
            var webm = SelectBestWebmAAudio(formats);
            return $"-f {webm.Id} {videoUrl}";
        }

        var video = SelectVideo(formats, qualityToSelect);
        var audio = SelectBestM4AAudio(formats);

        return $"-f {video.Id}+{audio.Id} {videoUrl}";
    }
}
