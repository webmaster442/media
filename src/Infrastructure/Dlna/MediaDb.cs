
using System.Net.Mime;

using Media.Dto.Internals;

namespace Media.Infrastructure.Dlna;

internal sealed class MediaDb
{
    private readonly Dictionary<string, string> _audioMimes;
    private readonly Dictionary<string, string> _videoMimes;
    private readonly Dictionary<string, string> _imageMimes;

    private readonly List<MediaRoute> _images;
    private readonly List<MediaRoute> _videos;
    private readonly List<MediaRoute> _audios;

    public MediaDb()
    {
        _imageMimes = new Dictionary<string, string>()
        {
            { ".jpg", MediaTypeNames.Image.Jpeg },
            { ".jpeg", MediaTypeNames.Image.Jpeg },
            { ".png", MediaTypeNames.Image.Png },
            { ".bmp", MediaTypeNames.Image.Bmp },
            { ".gif", MediaTypeNames.Image.Gif },
            { ".tiff", MediaTypeNames.Image.Tiff },
            { ".webp", MediaTypeNames.Image.Webp }
        };
        _audioMimes = new Dictionary<string, string>()
        {
            { ".aac", "audio/aac" },
            { ".ac3", "audio/ac3" },
            { ".aiff", "audio/x-aiff" },
            { ".dts", "audio/vnd.dts" },
            { ".flac", "audio/flac" },
            { ".m4a", "audio/mp4" },
            { ".m4b", "audio/mp4" },
            { ".mp3", "audio/mpeg" },
            { ".oga", "audio/ogg" },
            { ".opus", "audio/ogg" },
            { ".ogg", "audio/ogg" },
            { ".wav", "audio/wav" },
            { ".weba", "audio/webm" },
            { ".wma", "audio/x-ms-wma" },
        };
        _videoMimes = new Dictionary<string, string>()
        {
            { ".mp4", "video/mp4" },
            { ".m4v", "video/mp4" },
            { ".ogv", "video/ogg" },
            { ".webm", "video/webm" },
            { ".wmv", "video/x-ms-wmv" },
            { ".mkv", "video/matroska" },
            { ".mpg", "video/mpeg" },
            { ".ts", "video/mp2t" },
            { ".m2t", "video/mp2t" },
            { ".m2ts", "video/m2ts" },
            { ".mts", "video/m2ts" },
            { ".avi", "video/x-msvideo" },
        };
        _audios = new List<MediaRoute>();
        _videos = new List<MediaRoute>();
        _images = new List<MediaRoute>();
    }

    public void ScanFolder(string folder)
    {
        var files = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var extension = Path.GetExtension(file);
            if (_imageMimes.ContainsKey(extension))
            {
                _images.Add(new MediaRoute
                {
                    FilePath = file,
                    FileUrl = $"/{Path.GetFileName(file)}",
                    MimeType = _imageMimes[extension],
                });
            }
            else if (_audioMimes.ContainsKey(extension))
            {
                _audios.Add(new MediaRoute
                {
                    FilePath = file,
                    FileUrl = $"/{Path.GetFileName(file)}",
                    MimeType = _audioMimes[extension],
                });
            }
            else if (_videoMimes.ContainsKey(extension))
            {
                _videos.Add(new MediaRoute
                {
                    FilePath = file,
                    FileUrl = $"/{Path.GetFileName(file)}",
                    MimeType = _videoMimes[extension],
                });
            }
        }
    }

    public IReadOnlyList<MediaRoute> Images
        => _images;

    public IReadOnlyList<MediaRoute> Audios
        => _audios;

    public IReadOnlyList<MediaRoute> Videos
        => _videos;

    public IEnumerable<MediaRoute> All
        => _images.Concat(_audios)
                  .Concat(_videos)
                  .OrderBy(x => x.FileUrl);

    public int Count
        => _images.Count + _videos.Count + _audios.Count;
}
