using System.Net;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Http;

public sealed class MacAuthorizer : Logging, IHttpAuthorizationMethod
{
    private readonly Dictionary<string, object?> _macs =
      new Dictionary<string, object?>();

    public MacAuthorizer(IEnumerable<string> macs)
    {
        ArgumentNullException.ThrowIfNull(macs);
        foreach (var m in macs)
        {
            var mac = m.ToUpperInvariant().Trim();
            if (!IP.IsAcceptedMAC(mac))
            {
                throw new FormatException("Invalid MAC supplied");
            }
            _macs.Add(mac, null);
        }
    }

    public bool Authorize(IHeaders headers, IPEndPoint endPoint, string? mac)
    {
        if (string.IsNullOrEmpty(mac))
        {
            return false;
        }

        var rv = _macs.ContainsKey(mac);
        DebugFormat(!rv ? "Rejecting {0}. Not in MAC whitelist" : "Accepted {0} via MAC whitelist", mac);
        return rv;
    }
}