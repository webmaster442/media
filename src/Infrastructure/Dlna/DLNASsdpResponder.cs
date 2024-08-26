// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Net;
using System.Net.Sockets;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Media.Infrastructure.Dlna;

public class DLNASsdpResponder
{
    private readonly ILogger _logger;
    private readonly IEnumerable<(IPAddress adress, IPAddress mask)> _ipAdresses;
    private readonly int _httpServerPort;
    private const string MulticastAddress = "239.255.255.250";
    private const int MulticastPort = 1900;
    private readonly Dictionary<string, string> _stUSns;

    public DLNASsdpResponder(ILogger logger,
                             int serverPort,
                             IEnumerable<(IPAddress adress, IPAddress mask)> httpServerIps)
    {
        _httpServerPort = serverPort;
        _logger = logger;
        _ipAdresses = httpServerIps;
        _stUSns = new Dictionary<string, string>
        {
            { "uuid:4a8f3b30-d62f-40ed-b003-0719db08fdad", "uuid:4a8f3b30-d62f-40ed-b003-0719db08fdad" },
            { "upnp:rootdevice", "uuid:4a8f3b30-d62f-40ed-b003-0719db08fdad::upnp:rootdevice" },
            { "urn:schemas-upnp-org:device:MediaServer:1", "uuid:4a8f3b30-d62f-40ed-b003-0719db08fdad::urn:schemas-upnp-org:device:MediaServer:1" },
            { "urn:schemas-upnp-org:service:ContentDirectory:1", "uuid:4a8f3b30-d62f-40ed-b003-0719db08fdad::urn:schemas-upnp-org:service:ContentDirectory:1" }
        };
    }

    private static bool IsSameNetwork(IPAddress a, IPAddress b, IPAddress mask)
    {
        static uint ToUint(IPAddress address)
        {
            byte[] bytes = address.GetAddressBytes();
            return BitConverter.ToUInt32(bytes, 0);
        }

        uint netA = ToUint(a) & ToUint(mask);
        uint netB = ToUint(b) & ToUint(mask);
        return netA == netB;
    }


    private async Task SendSSDPResponse(UdpClient client, IPEndPoint destinationEndPoint)
    {
        var locations = _ipAdresses
            .Where(ip => IsSameNetwork(ip.adress, destinationEndPoint.Address, ip.mask))
            .Select(ip => $"http://{ip.adress}:{_httpServerPort}");

        foreach (var serverLocation in locations)
        {
            foreach (var stusn in _stUSns)
            {
                var response = $"""
                HTTP/1.1 200 OK
                CACHE-CONTROL: max-age=1800
                DATE: {DateTime.UtcNow:r}
                ST: {stusn.Key}
                USN: {stusn.Value}
                EXT:
                LOCATION: {serverLocation}/rootDesc.xml
                SERVER: Media Cli DLNA Server/1.0 UPnP/1.0
                Content-Length: 0

                """;

                byte[] responseBytes = Encoding.UTF8.GetBytes(response);

               await client.SendAsync(responseBytes, responseBytes.Length, destinationEndPoint);
            }
            _logger.LogInformation("Sent SSDP response to {Adress}:{Port}",
                       destinationEndPoint.Address,
                       destinationEndPoint.Port);
        }
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var udpClient = new UdpClient();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse(MulticastAddress), IPAddress.Any));
        udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
        udpClient.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);

        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, MulticastPort);
        udpClient.Client.Bind(localEndPoint);

        _logger.LogInformation("Listening for SSDP requests...");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                UdpReceiveResult receivedResult = await udpClient.ReceiveAsync();
                string receivedMessage = Encoding.UTF8.GetString(receivedResult.Buffer);
                _logger.LogInformation("Received message: {message}", receivedMessage);

                if (receivedMessage.Contains("M-SEARCH")
                    && receivedMessage.Contains("ssdp:discover"))
                {
                    await SendSSDPResponse(udpClient, receivedResult.RemoteEndPoint);
                }
            }
        }
        catch (ObjectDisposedException)
        {
            _logger.LogInformation("UDP client has been closed.");
        }
        finally
        {
            udpClient.Close();
        }
    }
}
