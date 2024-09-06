using System.Net;

using NMaier.SimpleDlna.Server.Interfaces;

namespace NMaier.SimpleDlna.Server.Http;

public interface IHttpAuthorizationMethod
{
    /// <summary>
    ///   Checks if a request is authorized.
    /// </summary>
    /// <param name="headers">Client supplied HttpHeaders.</param>
    /// <param name="endPoint">Client EndPoint</param>
    /// <returns>true if authorized</returns>
    bool Authorize(IHeaders headers, IPEndPoint endPoint);
}