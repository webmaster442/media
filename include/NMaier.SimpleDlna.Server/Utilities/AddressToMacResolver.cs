using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Server.Utilities;

internal sealed partial class AddressToMacResolver : Logging
{
    [GeneratedRegex(@"(?:[0-9A-F]{2}:){5}[0-9A-F]{2}", RegexOptions.Compiled)]
    private static partial Regex RegMac();

    private readonly ConcurrentDictionary<IPAddress, MACInfo> _cache =
      new ConcurrentDictionary<IPAddress, MACInfo>();

    public static bool IsAcceptedMac(string mac)
    {
        if (string.IsNullOrWhiteSpace(mac))
        {
            return false;
        }
        mac = mac.Trim().ToUpperInvariant();
        return RegMac().IsMatch(mac);
    }

    public string? Resolve(IPAddress ip)
    {
        try
        {
            if (ip.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new NotSupportedException(
                  "Addresses other than IPV4 are not supported");
            }
            if (_cache.TryGetValue(ip, out MACInfo? info) && info.Fresh > DateTime.Now)
            {
                DebugFormat("From Cache: {0} -> {1}", ip, info.MAC ?? "<UNKNOWN>");
                return info.MAC;
            }
            var raw = new byte[6];
            var length = 6u;

            var addr = ip.AddressFamily == AddressFamily.InterNetwork
                ? BitConverter.ToUInt32(ip.GetAddressBytes())
                : throw new NotSupportedException("Addresses other than IPV4 are not supported");

            string? mac = null;

            try
            {
                if (SafeNativeMethods.SendARP(addr, 0, raw, ref length) == 0)
                {
                    mac = $"{raw[0]:X}:{raw[1]:X}:{raw[2]:X}:{raw[3]:X}:{raw[4]:X}:{raw[5]:X}";
                }
            }
            catch (DllNotFoundException)
            {
                // ignore
            }
            _cache.TryAdd(ip, new MACInfo
            {
                MAC = mac,
                Fresh = DateTime.Now.AddMinutes(mac != null ? 10 : 1)
            });
            DebugFormat("Retrieved: {0} -> {1}", ip, mac ?? "<UNKNOWN>");
            return mac;
        }
        catch (Exception ex)
        {
            Warn($"Failed to resolve {ip} to MAC", ex);
            return null;
        }
    }

    private record class MACInfo
    {
        public DateTime Fresh { get; init; }

        public string? MAC { get; init; }
    }
}