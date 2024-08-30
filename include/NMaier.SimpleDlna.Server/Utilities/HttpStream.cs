using System.Net;
using System.Net.Http.Headers;
using System.Reflection;

using log4net;

namespace NMaier.SimpleDlna.Server.Utilities;

public class HttpStream : Stream
{
    private const int BUFFER_SIZE = 1 << 10;

    private const long SMALL_SEEK = 1 << 9;

    private const int TIMEOUT = 30000;

    private static readonly ILog logger =
      LogManager.GetLogger(typeof(HttpStream));

    public static readonly string UserAgent = GenerateUserAgent();

    private readonly Uri? _referrer;

    private readonly Uri _uri;

    private Stream? _bufferedStream;

    private long? _length;

    private long _position;

    private HttpWebRequest? _request;

    private HttpWebResponse? _response;

    private Stream? _responseStream;

    public HttpStream(Uri uri)
      : this(uri, null)
    {
    }

    public HttpStream(Uri uri, Uri? referrer)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }
        _uri = uri;
        _referrer = referrer;
    }

    public override bool CanRead => true;

    public override bool CanSeek
    {
        get
        {
            if (Length <= 0)
            {
                return false;
            }

            EnsureResponse();
            var ranges = _response?.Headers.Get("Accept-Ranges");
            if (!string.IsNullOrEmpty(ranges) &&
                ranges.Equals("none", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }
    }

    public override bool CanTimeout => true;

    public override bool CanWrite => false;

    public string ContentType
    {
        get
        {
            EnsureResponse();
            return _response?.ContentType ?? string.Empty;
        }
    }

    public DateTime LastModified
    {
        get
        {
            EnsureResponse();
            return _response?.LastModified ?? DateTime.Now;
        }
    }

    public override long Length
    {
        get
        {
            if (!_length.HasValue)
            {
                OpenAt(0, HttpMethod.HEAD);
                _length = _response?.ContentLength;
            }
            if (_length == null || _length.Value < 0)
            {
                throw new IOException("Stream does not feature a length");
            }
            return _length.Value;
        }
    }

    public override long Position
    {
        get { return _position; }
        set { Seek(value, SeekOrigin.Begin); }
    }

    public Uri Uri => new Uri(_uri.ToString());

    private void EnsureResponse()
    {
        if (_response != null)
        {
            return;
        }
        OpenAt(0, HttpMethod.HEAD);
    }

    private static string GenerateUserAgent()
    {
        var os = Environment.OSVersion;
        string pstring;
        switch (os.Platform)
        {
            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
                pstring = "WIN";
                break;

            default:
                pstring = "Unix";
                break;
        }
        return string.Format(
          "sdlna/{4}.{5} ({0}{1} {2}.{3}) like curl/7.3 like wget/1.0",
          pstring,
          nint.Size * 8,
          os.Version.Major,
          os.Version.Minor,
          Assembly.GetExecutingAssembly().GetName().Version?.Major,
          Assembly.GetExecutingAssembly().GetName().Version?.Minor
          );
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_bufferedStream != null)
            {
                _bufferedStream.Dispose();
                _bufferedStream = null;
            }
            if (_responseStream != null)
            {
                _responseStream.Dispose();
                _responseStream = null;
            }
            _response = null;
            _request = null;
        }

        base.Dispose(disposing);
    }

    protected void OpenAt(long offset, HttpMethod method)
    {
        if (offset < 0)
        {
            throw new IOException("Position cannot be negative");
        }
        if (offset > 0 && method == HttpMethod.HEAD)
        {
            throw new ArgumentException(
              "Cannot use a position (seek) with HEAD request");
        }
        Close();
        Dispose();

        _request = (HttpWebRequest)WebRequest.Create(_uri);
        _request.Method = method.ToString();
        if (_referrer != null)
        {
            _request.Referer = _referrer.ToString();
        }
        _request.AllowAutoRedirect = true;
        _request.Timeout = TIMEOUT * 1000;
        _request.UserAgent = UserAgent;
        if (offset > 0)
        {
            _request.AddRange(offset);
        }
        _response = (HttpWebResponse)_request.GetResponse();
        if (method != HttpMethod.HEAD)
        {
            _responseStream = _response.GetResponseStream();
            if (_responseStream == null)
            {
                throw new IOException("Didn't get a response stream");
            }
            _bufferedStream = new BufferedStream(_responseStream, BUFFER_SIZE);
        }
        if (offset > 0 && _response.StatusCode != HttpStatusCode.PartialContent)
        {
            throw new IOException(
              "Failed to open the http stream at a specific position");
        }
        if (offset == 0 && _response.StatusCode != HttpStatusCode.OK)
        {
            throw new IOException("Failed to open the http stream");
        }
        logger.InfoFormat("Opened {0} {1} at {2}", method, _uri, offset);
    }

    public override void Close()
    {
        _bufferedStream?.Close();
        _responseStream?.Close();
        _response?.Close();
        base.Close();
    }

    public override void Flush()
    {
        Dispose(true);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        try
        {
            if (_responseStream == null)
            {
                OpenAt(_position, HttpMethod.GET);
            }
            var read = _bufferedStream?.Read(buffer, offset, count) ?? 0;
            if (read > 0)
            {
                _position += read;
            }
            return read;
        }
        catch (Exception ex)
        {
            logger.Error("Failed to read", ex);
            throw;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        logger.DebugFormat("Seek to {0}, {1} requested", offset, origin);
        var np = 0L;
        switch (origin)
        {
            case SeekOrigin.Begin:
                np = offset;
                break;

            case SeekOrigin.Current:
                np = _position + offset;
                break;

            case SeekOrigin.End:
                np = Length + np;
                break;
        }
        if (np < 0 || np >= Length)
        {
            throw new IOException("Invalid seek; out of stream bounds");
        }
        var off = _position - np;
        if (off == 0)
        {
            logger.Debug("No seek required");
        }
        else
        {
            if (_response != null && off > 0 && off < SMALL_SEEK)
            {
                var buf = new byte[off];
                _bufferedStream?.Read(buf, 0, (int)off);
                logger.DebugFormat("Did a small seek of {0}", off);
            }
            else
            {
                OpenAt(np, HttpMethod.GET);
                logger.DebugFormat("Did a long seek of {0}", off);
            }
        }
        _position = np;
        logger.DebugFormat("Successfully sought to {0}", _position);
        return _position;
    }

    public override void SetLength(long value)
    {
        _length = value;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
}