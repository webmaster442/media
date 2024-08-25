// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Net;
using System.Net.Sockets;

namespace Media.Infrastructure.Dlna;

public class DLNASsdpResponder
{
    private readonly ILogger<DLNASsdpResponder> _logger;
    private readonly IEnumerable<string> _serverLocations;
    private const string MulticastAddress = "239.255.255.250";
    private const int MulticastPort = 1900;

    public DLNASsdpResponder(ILogger<DLNASsdpResponder> logger, IEnumerable<string> serverLocations)
    {
        _logger = logger;
        _serverLocations = serverLocations;
    }

    private async Task SendSSDPResponse(UdpClient client, IPEndPoint destinationEndPoint)
    {
        foreach (var serverLocation in _serverLocations)
        {
            var response = $"""
                HTTP/1.1 200 OK
                CACHE-CONTROL: max-age=1800
                DATE: {DateTime.UtcNow:r}
                ST: urn:schemas-upnp-org:service:ContentDirectory:1
                USN: uuid:4a8f3b30-d62f-40ed-b003-0719db08fdad::urn:schemas-upnp-org:service:ContentDirectory:1
                EXT:
                LOCATION: {serverLocation}/rootDesc.xml
                SERVER: Media Cli DLNA Server/1.0 UPnP/1.0
                Content-Length: 0

                """;

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);

            await client.SendAsync(responseBytes, responseBytes.Length, destinationEndPoint);

            _logger.LogInformation("Sent SSDP response to {Adress}:{Port}",
                                   destinationEndPoint.Address,
                                   destinationEndPoint.Port);
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var udpClient = new UdpClient();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.JoinMulticastGroup(IPAddress.Parse(MulticastAddress));

        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, MulticastPort);
        udpClient.Client.Bind(localEndPoint);

        _logger.LogInformation("Listening for SSDP requests...");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var receivedResult = await udpClient.ReceiveAsync();
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
