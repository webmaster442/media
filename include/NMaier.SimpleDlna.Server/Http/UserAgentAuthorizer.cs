using System.Net;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Http;

public sealed class UserAgentAuthorizer : Logging, IHttpAuthorizationMethod
{
    private readonly Dictionary<string, object?> userAgents =
      new Dictionary<string, object?>();

    public UserAgentAuthorizer(IEnumerable<string> userAgents, ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(userAgents);
        foreach (var u in userAgents)
        {
            if (string.IsNullOrEmpty(u))
            {
                throw new FormatException("Invalid User-Agent supplied");
            }
            this.userAgents.Add(u, null);
        }
    }

    public bool Authorize(IHeaders headers, IPEndPoint endPoint)
    {
        ArgumentNullException.ThrowIfNull(headers);
        if (!headers.TryGetValue("User-Agent", out string? ua))
        {
            return false;
        }
        if (string.IsNullOrEmpty(ua))
        {
            return false;
        }
        var rv = userAgents.ContainsKey(ua);
        Logger.LogDebug(!rv ? "Rejecting {ua}. Not in User-Agent whitelist" : "Accepted {ua} via User-Agent whitelist", ua);
        return rv;
    }
}