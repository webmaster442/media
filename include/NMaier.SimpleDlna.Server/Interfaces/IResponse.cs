using NMaier.SimpleDlna.Server.Http;

namespace NMaier.SimpleDlna.Server.Interfaces;

internal interface IResponse
{
    Stream Body { get; }

    IHeaders Headers { get; }

    HttpCode Status { get; }
}