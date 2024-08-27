using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Server.Interfaces;

namespace NMaier.SimpleDlna.Server.Responses;

internal sealed class FileResponse : IResponse
{
    private readonly FileInfo _body;

    public FileResponse(HttpCode aStatus, FileInfo aBody)
      : this(aStatus, "text/html; charset=utf-8", aBody)
    {
    }

    public FileResponse(HttpCode aStatus, string aMime, FileInfo aBody)
    {
        Status = aStatus;
        _body = aBody;

        Headers["Content-Type"] = aMime;
        Headers["Content-Length"] = _body.Length.ToString();
    }

    public Stream Body => _body.OpenRead();

    public IHeaders Headers { get; } = new ResponseHeaders();

    public HttpCode Status { get; }
}