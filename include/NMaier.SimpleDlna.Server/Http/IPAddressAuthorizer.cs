using System.Net;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Http;

public sealed class IPAddressAuthorizer : Logging, IHttpAuthorizationMethod
{
    private readonly Dictionary<IPAddress, object?> _ips =
      new Dictionary<IPAddress, object?>();

    public IPAddressAuthorizer(IEnumerable<IPAddress> addresses)
    {
        ArgumentNullException.ThrowIfNull(addresses);
        foreach (var ip in addresses)
        {
            _ips.Add(ip, null);
        }
    }

    public IPAddressAuthorizer(IEnumerable<string> addresses)
      : this(from a in addresses select IPAddress.Parse(a))
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
        DebugFormat(!rv ? "Rejecting {0}. Not in IP whitelist" : "Accepted {0} via IP whitelist", addr);
        return rv;
    }
}