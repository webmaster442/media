using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Http;

internal sealed partial class HttpClient : Logging, IRequest, IDisposable
{
    private const uint BEGIN_TIMEOUT = 30;

    private const int BUFFER_SIZE = 1 << 16;

    private const string CRLF = "\r\n";

    private static readonly IHandler Error403 =
      new StaticHandler(new StringResponse(
                          HttpCode.Denied,
                          "<!doctype html><title>Access denied!</title><h1>Access denied!</h1><p>You're not allowed to access the requested resource.</p>"
                          )
        );

    private static readonly IHandler Error404 =
      new StaticHandler(new StringResponse(
                          HttpCode.NotFound,
                          "<!doctype html><title>Not found!</title><h1>Not found!</h1><p>The requested resource was not found!</p>"
                          )
        );

    private static readonly IHandler Error416 =
      new StaticHandler(new StringResponse(
                          HttpCode.RangeNotSatisfiable,
                          "<!doctype html><title>Requested Range not satisfiable!</title><h1>Requested Range not satisfiable!</h1><p>Nice try, but do not try again :p</p>"
                          )
        );

    private static readonly IHandler Error500 =
      new StaticHandler(new StringResponse(
                          HttpCode.InternalError,
                          "<!doctype html><title>Internal Server Error</title><h1>Internal Server Error</h1><p>Something is very rotten in the State of Denmark!</p>"
                          )
        );

    private readonly byte[] _buffer = new byte[2048];

    private readonly TcpClient _client;

    private readonly HttpServer _owner;

    private readonly uint _readTimeout =
      (uint)TimeSpan.FromMinutes(1).TotalSeconds;

    private readonly NetworkStream _stream;

    private readonly uint _writeTimeout =
      (uint)TimeSpan.FromMinutes(180).TotalSeconds;

    private uint _bodyBytes;

    private bool _hasHeaders;

    private DateTime _lastActivity;

    private MemoryStream _readStream;

    private uint requestCount;

    private IResponse _response;

    private HttpStates _state;

    public HttpClient(HttpServer aOwner, TcpClient aClient)
    {
        State = HttpStates.Accepted;
        _lastActivity = DateTime.Now;

        _owner = aOwner;
        _client = aClient;
        _stream = _client.GetStream();

        RemoteEndpoint = _client.Client.RemoteEndPoint as IPEndPoint ?? throw new InvalidOperationException();
        LocalEndPoint = _client.Client.LocalEndPoint as IPEndPoint ?? throw new InvalidOperationException();
    }

    private HttpStates State
    {
        set
        {
            _lastActivity = DateTime.Now;
            _state = value;
        }
    }

    public bool IsATimeout
    {
        get
        {
            var diff = (DateTime.Now - _lastActivity).TotalSeconds;
            return _state switch
            {
                HttpStates.Accepted or HttpStates.ReadBegin or HttpStates.WriteBegin => diff > BEGIN_TIMEOUT,
                HttpStates.Reading => diff > _readTimeout,
                HttpStates.Writing => diff > _writeTimeout,
                HttpStates.Closed => true,
                _ => throw new InvalidOperationException("Invalid state"),
            };
        }
    }

    public void Dispose()
    {
        Close();
        _readStream?.Dispose();
    }

    public string? Body { get; private set; }

    public IHeaders Headers { get; } = new Headers();

    public IPEndPoint LocalEndPoint { get; }

    public string? Method { get; private set; }

    public string Path { get; private set; }

    public IPEndPoint RemoteEndpoint { get; }

    private long GetContentLengthFromStream(Stream responseBody)
    {
        long contentLength = -1;
        try
        {
            if (!_response.Headers.TryGetValue("Content-Length", out string? clf) ||
                !long.TryParse(clf, out contentLength))
            {
                contentLength = responseBody.Length - responseBody.Position;
                if (contentLength < 0)
                {
                    throw new InvalidDataException();
                }
                _response.Headers["Content-Length"] = contentLength.ToString();
            }
        }
        catch (Exception ex)
        {
            Warn(ex);
            // ignored
        }
        return contentLength;
    }

    private Stream ProcessRanges(IResponse rangedResponse, ref HttpCode status)
    {
        var responseBody = rangedResponse.Body;
        var contentLength = GetContentLengthFromStream(responseBody);
        try
        {
            string? ar;
            if (status != HttpCode.Ok && contentLength > 0 ||
                !Headers.TryGetValue("Range", out ar))
            {
                return responseBody;
            }
            var m = BytesRegex().Match(ar);
            if (!m.Success)
            {
                throw new InvalidDataException("Not parsed!");
            }
            var totalLength = contentLength;
            long start;
            long end;
            if (!long.TryParse(m.Groups[1].Value, out start) || start < 0)
            {
                throw new InvalidDataException("Not parsed");
            }
            if (m.Groups.Count != 3 ||
                !long.TryParse(m.Groups[2].Value, out end) ||
                end <= start || end >= totalLength)
            {
                end = totalLength - 1;
            }
            if (start >= end)
            {
                responseBody.Close();
                rangedResponse = Error416.HandleRequest(this);
                return rangedResponse.Body;
            }

            if (start > 0)
            {
                responseBody.Seek(start, SeekOrigin.Current);
            }
            contentLength = end - start + 1;
            rangedResponse.Headers["Content-Length"] = contentLength.ToString();
            rangedResponse.Headers.Add(
              "Content-Range",
              $"bytes {start}-{end}/{totalLength}"
              );
            status = HttpCode.Partial;
        }
        catch (Exception ex)
        {
            Warn($"{this} - Failed to process range request!", ex);
        }
        return responseBody;
    }

    private void Read()
    {
        try
        {
            _stream.BeginRead(_buffer, 0, _buffer.Length, ReadCallback, 0);
        }
        catch (IOException ex)
        {
            Warn($"{this} - Failed to BeginRead", ex);
            Close();
        }
    }

    private void ReadCallback(IAsyncResult result)
    {
        if (_state == HttpStates.Closed)
        {
            return;
        }

        State = HttpStates.Reading;

        try
        {
            var read = _stream.EndRead(result);
            if (read < 0)
            {
                throw new HttpException("Client did not send anything");
            }
            DebugFormat("{0} - Read {1} bytes", this, read);
            _readStream.Write(_buffer, 0, read);
            _lastActivity = DateTime.Now;
        }
        catch (Exception)
        {
            if (!IsATimeout)
            {
                WarnFormat("{0} - Failed to read data", this);
                Close();
            }
            return;
        }

        try
        {
            if (!_hasHeaders)
            {
                _readStream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(_readStream);
                for (var line = reader.ReadLine();
                  line != null;
                  line = reader.ReadLine())
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        _hasHeaders = true;
                        _readStream = StreamManager.GetStream();
                        if (Headers.ContainsKey("content-length") &&
                            uint.TryParse(Headers["content-length"], out _bodyBytes))
                        {
                            if (_bodyBytes > 1 << 20)
                            {
                                throw new IOException("Body too long");
                            }
                            var ascii = Encoding.ASCII.GetBytes(reader.ReadToEnd());
                            _readStream.Write(ascii, 0, ascii.Length);
                            DebugFormat("Must read body bytes {0}", _bodyBytes);
                        }
                        break;
                    }
                    if (Method == null)
                    {
                        var parts = line.Split(new[] { ' ' }, 3);
                        Method = parts[0].Trim().ToUpperInvariant();
                        Path = parts[1].Trim();
                        DebugFormat("{0} - {1} request for {2}", this, Method, Path);
                    }
                    else
                    {
                        var parts = line.Split(new[] { ':' }, 2);
                        Headers[parts[0]] = Uri.UnescapeDataString(parts[1]).Trim();
                    }
                }
            }
            if (_bodyBytes != 0 && _bodyBytes > _readStream.Length)
            {
                DebugFormat(
                  "{0} - Bytes to go {1}", this, _bodyBytes - _readStream.Length);
                Read();
                return;
            }
            using (_readStream)
            {
                Body = Encoding.UTF8.GetString(_readStream.ToArray());
                Debug(Body);
                Debug(Headers);
            }
            SetupResponse();
        }
        catch (Exception ex)
        {
            Warn($"{this} - Failed to process request", ex);
            _response = Error500.HandleRequest(this);
            SendResponse();
        }
    }

    private void ReadNext()
    {
        Method = null;
        Headers.Clear();
        _hasHeaders = false;
        Body = null;
        _bodyBytes = 0;
        _readStream = StreamManager.GetStream();

        ++requestCount;
        State = HttpStates.ReadBegin;

        Read();
    }

    private void SendResponse()
    {
        var statusCode = _response.Status;
        var responseBody = ProcessRanges(_response, ref statusCode);
        var responseStream = new ConcatenatedStream();
        try
        {
            var headerBlock = new StringBuilder();
            headerBlock.AppendFormat(
              "HTTP/1.1 {0} {1}\r\n",
              (uint)statusCode,
              HttpPhrases.Phrases[statusCode]
              );
            headerBlock.Append(_response.Headers.HeaderBlock);
            headerBlock.Append(CRLF);

            var headerStream = new MemoryStream(
              Encoding.ASCII.GetBytes(headerBlock.ToString()));
            responseStream.AddStream(headerStream);
            if (Method != "HEAD" && responseBody != null)
            {
                responseStream.AddStream(responseBody);
                responseBody = null;
            }
            InfoFormat("{0} - {1} response for {2}", this, (uint)statusCode, Path);
            _state = HttpStates.Writing;
            using var sp = new StreamPump(responseStream, _stream, BUFFER_SIZE);
            sp.Pump((pump, result) =>
            {
                pump.Input.Close();
                pump.Input.Dispose();
                if (result == StreamPumpResult.Delivered)
                {
                    DebugFormat("{0} - Done writing response", this);

                    if (Headers.TryGetValue("connection", out string? conn) 
                    && conn.Equals("KEEP-ALIVE", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ReadNext();
                        return;
                    }
                }
                else
                {
                    DebugFormat("{0} - Client aborted connection", this);
                }
                Close();
            });
        }
        catch (Exception)
        {
            responseStream.Dispose();
            throw;
        }
        finally
        {
            responseBody?.Dispose();
        }
    }

    private void SetupResponse()
    {
        State = HttpStates.WriteBegin;
        try
        {
            if (!_owner.AuthorizeClient(this))
            {
                throw new HttpStatusException(HttpCode.Denied);
            }
            if (string.IsNullOrEmpty(Path))
            {
                throw new HttpStatusException(HttpCode.NotFound);
            }
            var handler = _owner.FindHandler(Path);
            if (handler == null)
            {
                throw new HttpStatusException(HttpCode.NotFound);
            }
            _response = handler.HandleRequest(this);
            if (_response == null)
            {
                throw new ArgumentException("Handler did not return a response");
            }
        }
        catch (HttpStatusException ex)
        {
#if DEBUG
            Warn(string.Format("{0} - Got a {2}: {1}", this, Path, ex.Code), ex);
#else
    InfoFormat("{0} - Got a {2}: {1}", this, Path, ex.Code);
#endif
            switch (ex.Code)
            {
                case HttpCode.NotFound:
                    _response = Error404.HandleRequest(this);
                    break;
                case HttpCode.Denied:
                    _response = Error403.HandleRequest(this);
                    break;
                case HttpCode.InternalError:
                    _response = Error500.HandleRequest(this);
                    break;
                default:
                    _response = new StaticHandler(new StringResponse(
                                                   ex.Code,
                                                   "text/plain",
                                                   ex.Message
                                                   )).HandleRequest(this);
                    break;
            }
        }
        catch (Exception ex)
        {
            Warn($"{this} - Failed to process response", ex);
            _response = Error500.HandleRequest(this);
        }
        SendResponse();
    }

    internal void Close()
    {
        State = HttpStates.Closed;

        DebugFormat("{0} - Closing connection after {1} requests", this, requestCount);
        try
        {
            _client.Close();
        }
        catch (Exception ex)
        {
            Warn(ex);
            // ignored
        }
        _owner.RemoveClient(this);
        if (_stream != null)
        {
            try
            {
                _stream.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }

    public void Start()
    {
        ReadNext();
    }

    public override string ToString()
    {
        return RemoteEndpoint.ToString();
    }

    internal enum HttpStates
    {
        Accepted,
        Closed,
        ReadBegin,
        Reading,
        WriteBegin,
        Writing
    }

    [GeneratedRegex(@"^bytes=(\d+)(?:-(\d+)?)?$", RegexOptions.Compiled)]
    private static partial Regex BytesRegex();
}
