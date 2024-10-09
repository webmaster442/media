using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NMaier.SimpleDlna.Server.Utilities;

public static class IP
{
    private static bool _warned;

    public static IEnumerable<IPAddress> AllIPAddresses
    {
        get
        {
            try
            {
                return GetIPsDefault().ToArray();
            }
            catch (Exception ex)
            {
                if (!_warned)
                {
                    Debug.WriteLine("Failed to retrieve IP addresses the usual way, falling back to naive mode: {0}", ex);
                    _warned = true;
                }
                return GetIPsFallback();
            }
        }
    }

    public static IEnumerable<IPAddress> ExternalIPAddresses => from i in AllIPAddresses
                                                                where !IPAddress.IsLoopback(i)
                                                                select i;

    private static IEnumerable<IPAddress> GetIPsDefault()
    {
        var returned = false;
        foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            var props = adapter.GetIPProperties();
            var gateways = from ga in props.GatewayAddresses
                           where !ga.Address.Equals(IPAddress.Any)
                           select true;
            if (!gateways.Any())
            {
                Debug.WriteLine("Skipping {0}. No gateways", props);
                continue;
            }
            Debug.WriteLine("Using {0}", props);
            foreach (var uni in props.UnicastAddresses)
            {
                var address = uni.Address;
                if (address.AddressFamily != AddressFamily.InterNetwork)
                {
                    Debug.WriteLine("Skipping {0}. Not IPv4", address);
                    continue;
                }
                Debug.WriteLine("Found {0}", address);
                returned = true;
                yield return address;
            }
        }
        if (!returned)
        {
            throw new ApplicationException("No IP");
        }
    }

    private static IEnumerable<IPAddress> GetIPsFallback()
    {
        var returned = false;
        foreach (var i in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (i.AddressFamily == AddressFamily.InterNetwork)
            {
                returned = true;
                yield return i;
            }
        }
        if (!returned)
        {
            throw new ApplicationException("No IP");
        }
    }
}