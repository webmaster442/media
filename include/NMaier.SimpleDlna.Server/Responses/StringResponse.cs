using System.Text;

using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Server.Interfaces;

namespace NMaier.SimpleDlna.Server.Responses;

internal class StringResponse : IResponse
{
    private readonly string _body;

    public StringResponse(HttpCode aStatus, string aBody)
      : this(aStatus, "text/html; charset=utf-8", aBody)
    {
    }

    public StringResponse(HttpCode aStatus, string aMime, string aBody)
    {
        Status = aStatus;
        _body = aBody;

        Headers["Content-Type"] = aMime;
        Headers["Content-Length"] = Encoding.UTF8.GetByteCount(_body).ToString();
    }

    public Stream Body => new MemoryStream(Encoding.UTF8.GetBytes(_body));

    public IHeaders Headers { get; } = new ResponseHeaders();

    public HttpCode Status { get; }
}