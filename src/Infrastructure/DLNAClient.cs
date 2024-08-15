using System.Net;
using System.Net.Sockets;

namespace Media.Infrastructure;

internal class DLNAClient
{
    public async Task<ISet<Uri>> SSDPQueryAsync(CancellationToken token)
    {
        HashSet<Uri> results = new HashSet<Uri>();

        IPEndPoint localEndPoint = new(IPAddress.Any, 6000);
        IPEndPoint multicastEndPoint = new(IPAddress.Parse("239.255.255.250"), 1900);//SSDP port

        using (Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpSocket.Bind(localEndPoint);
            udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastEndPoint.Address, IPAddress.Any));
            udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);

            var searchString = """
                M-SEARCH * HTTP/1.1
                HOST:239.255.255.250:1900
                MAN:"ssdp:discover"
                ST:ssdp:all
                MX:3


                """u8.ToArray();

            await udpSocket.SendToAsync(searchString, SocketFlags.None, multicastEndPoint, token);

            byte[] receiveBuffer = new byte[4096];

            int count = 0;
            while (count < 20)
            {
                token.ThrowIfCancellationRequested();
                count++;
                if (udpSocket.Available > 0)
                {
                    int receivedBytes = await udpSocket.ReceiveAsync(receiveBuffer, SocketFlags.None, token);
                    if (receivedBytes > 0)
                    {
                        string data = Encoding.UTF8.GetString(receiveBuffer, 0, receivedBytes);
                        if (data.IndexOf("LOCATION: ", StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            data = data.ChopOffBefore("LOCATION: ").ChopOffAfter(Environment.NewLine);
                        }
                        var url = new Uri(data);
                        results.Add(url);
                    }
                }
                else
                {
                    await Task.Delay(100, token);
                }
            }
        }
        return results;
    }
}
