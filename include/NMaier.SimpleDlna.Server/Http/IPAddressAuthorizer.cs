using System.Net;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Http;

public sealed class IPAddressAuthorizer : Logging, IHttpAuthorizationMethod
{
    private readonly Dictionary<IPAddress, object?> _ips =
      new Dictionary<IPAddress, object?>();

    public IPAddressAuthorizer(IEnumerable<IPAddress> addresses, ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(addresses);
        foreach (var ip in addresses)
        {
            _ips.Add(ip, null);
        }
    }

    public IPAddressAuthorizer(IEnumerable<string> addresses, ILoggerFactory loggerFactory)
      : this(from a in addresses select IPAddress.Parse(a), loggerFactory)
    {
    }

    public bool Authorize(IHeaders headers, IPEndPoint endPoint)
    {
        var addr = endPoint?.Address;
        if (addr == null)
        {
            return false;
        }
        var rv = _ips.ContainsKey(addr);
        Logger.LogDebug(!rv ? "Rejecting {addr}. Not in IP whitelist" : "Accepted {addr} via IP whitelist", addr);
        return rv;
    }
}