using System.Globalization;
using System.Text;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Types;

[Serializable]
public sealed class Subtitle : IMediaResource
{
    [NonSerialized]
    private static readonly string[] Exts =
    {
        ".srt", ".SRT",
        ".ass", ".ASS",
        ".ssa", ".SSA",
        ".sub", ".SUB",
        ".vtt", ".VTT"
    };

    [NonSerialized]
    private byte[]? _encodedText;

    private string? _text;
    private readonly ILogger<Subtitle> _logger;

    public Subtitle(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Subtitle>();
    }

    public Subtitle(FileInfo file, ILoggerFactory loggerFactory)
    {
        Load(file);
        _logger = loggerFactory.CreateLogger<Subtitle>();
    }

    public Subtitle(string text, ILoggerFactory loggerFactory)
    {
        _text = text;
        _logger = loggerFactory.CreateLogger<Subtitle>();
    }

    public bool HasSubtitle => !string.IsNullOrWhiteSpace(_text);

    public DateTime InfoDate => DateTime.UtcNow;

    public long? InfoSize
    {
        get
        {
            try
            {
                using (var s = CreateContentStream())
                {
                    return s.Length;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public IMediaCoverResource Cover
    {
        get { throw new NotImplementedException(); }
    }

    public string Id
    {
        get { return Path; }
        set { throw new NotImplementedException(); }
    }

    public DlnaMediaTypes MediaType
    {
        get { throw new NotImplementedException(); }
    }

    public string Path => "ad-hoc-subtitle:";

    public string PN => DlnaMaps.MainPN[Type];

    public IHeaders Properties
    {
        get
        {
            var rv = new RawHeaders { { "Type", Type.ToString() } };
            if (InfoSize.HasValue)
            {
                rv.Add("SizeRaw", InfoSize.Value.ToString());
                rv.Add("Size", InfoSize.Value.FormatFileSize());
            }
            rv.Add("Date", InfoDate.ToString(CultureInfo.InvariantCulture));
            rv.Add("DateO", InfoDate.ToString("o"));
            return rv;
        }
    }

    public string Title
    {
        get { throw new NotImplementedException(); }
    }

    public DlnaMime Type => DlnaMime.SubtitleSRT;

    public int CompareTo(IMediaItem? other)
    {
        throw new NotImplementedException();
    }

    public Stream CreateContentStream()
    {
        if (!HasSubtitle)
        {
            throw new NotSupportedException();
        }
        if (_encodedText == null)
        {
            _encodedText = _text != null ? Encoding.UTF8.GetBytes(_text) : Array.Empty<Byte>();
        }
        return new MemoryStream(_encodedText, false);
    }

    public bool Equals(IMediaItem? other)
    {
        throw new NotImplementedException();
    }

    public string ToComparableTitle()
    {
        throw new NotImplementedException();
    }

    private void Load(FileInfo file)
    {
        try
        {
            // Try external
            foreach (var i in Exts)
            {
                var sti = new FileInfo(
                  System.IO.Path.ChangeExtension(file.FullName, i));
                try
                {
                    if (!sti.Exists)
                    {
                        sti = new FileInfo(file.FullName + i);
                    }
                    if (!sti.Exists)
                    {
                        continue;
                    }
                    _text = FFmpeg.GetSubtitleSubrip(sti, _logger);
                    _logger.LogDebug("Loaded subtitle from {file}", sti.FullName);
                }
                catch (NotSupportedException)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Failed to get subtitle from {filename}, Exception: {ex}", sti.FullName, ex);
                }
            }
            try
            {
                _text = FFmpeg.GetSubtitleSubrip(file, _logger);
                _logger.LogDebug("Loaded subtitle from {file}", file.FullName);
            }
            catch (NotSupportedException ex)
            {
                _logger.LogDebug("Failed to get subtitle from {filename}, Exception: {ex}", file.FullName, ex);
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Failed to get subtitle from {filename}, Exception: {ex}", file.FullName, ex);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get subtitle from {filename}, Exception: {ex}", file.FullName, ex);
        }
    }
}