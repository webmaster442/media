// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;

using Media.Dto.Dlna;
using Media.Dto.Internals;

namespace Media.Infrastructure.Dlna;

internal sealed class DLNAClient : IDisposable
{
    private readonly HttpClient _client;
    private readonly XmlStringSerializer<Root> _rootSerializer;
    private readonly XmlStringSerializer<BrowseResponse> _browseResponseSerializer;
    private readonly XmlStringSerializer<BrowseRequest> _browseRequestSerializer;

    public DLNAClient()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
        _rootSerializer = new XmlStringSerializer<Root>();
        _browseResponseSerializer = new XmlStringSerializer<BrowseResponse>();
        _browseRequestSerializer = new XmlStringSerializer<BrowseRequest>();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    private async Task<ISet<Uri>> SSDPQueryAsync(CancellationToken token)
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
            while (count < 30)
            {
                token.ThrowIfCancellationRequested();
                count++;
                if (udpSocket.Available > 0)
                {
                    int receivedBytes = await udpSocket.ReceiveAsync(receiveBuffer, SocketFlags.None, token);
                    if (receivedBytes > 0)
                    {
                        string responseText = Encoding.UTF8.GetString(receiveBuffer, 0, receivedBytes);

                        var ssdpResponse = Parsers.ParseSSDPResponse(responseText);

                        if (ssdpResponse.ST.Contains("urn:schemas-upnp-org:device:MediaServer"))
                        {
                            var url = new Uri(ssdpResponse.Location);
                            results.Add(url);
                        }
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

    public async Task<IReadOnlyCollection<DlnaItem>> GetServersAsync(CancellationToken token)
    {
        var servers = new List<DlnaItem>();

        var ssdpResponses = await SSDPQueryAsync(token);
        foreach (var ssdpResponse in ssdpResponses)
        {
            token.ThrowIfCancellationRequested();
            var response = await _client.GetAsync(ssdpResponse);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(token);
            if (string.IsNullOrEmpty(content)
                || content.Contains("<netRemote")
                || content.Contains("<html"))
            {
                continue;
            }

            servers.Add(ProcessServerXml(ssdpResponse, content));
        }

        return servers;
    }

    public async Task<IReadOnlyCollection<DlnaItem>> Browse(string uri, string id, CancellationToken token)
    {
        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(uri),
            Method = HttpMethod.Post
        };
        request.Content = new StringContent(CreateBrowseXml(id), Encoding.UTF8, "text/xml");
        request.Headers.Clear();
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
        request.Headers.Add("SOAPAction", "urn:schemas-upnp-org:service:ContentDirectory:1#Browse");

        var response = await _client.SendAsync(request, token);
        response.EnsureSuccessStatusCode();

        var xml = await response.Content.ReadAsStringAsync(token);

        return ProcessEnvelopeXml(xml, uri);
    }

    private IReadOnlyCollection<DlnaItem> ProcessEnvelopeXml(string xml, string defaultUri)
    {
        BrowseResponse? response = _browseResponseSerializer.DeserializeSoap(xml, true);

        if (response != null)
        {
            List<DlnaItem> items = new();
            foreach (var objItem in response.Result.DIDLLite.Items)
            {
                if (objItem is DIDLLiteItem item)
                {
                    items.Add(new DlnaItem
                    {
                        Id = item.Id,
                        IsBrowsable = false,
                        IsServer = false,
                        Name = item.Title,
                        Uri = new Uri(item.Res.Value ?? string.Empty),
                    });
                }
                else if (objItem is DIDLLiteContainer container)
                {
                    items.Add(new DlnaItem
                    {
                        Id = container.Id,
                        IsBrowsable = true,
                        IsServer = false,
                        Name = container.Title,
                        Uri = new Uri(defaultUri),
                    });
                }
            }
            return items;
        }

        return Array.Empty<DlnaItem>();
    }

    private DlnaItem ProcessServerXml(Uri server, string content)
    {
        var data = _rootSerializer.Deserialize(content);
        var serverEntry = data?.Device.ServiceList?.Service.FirstOrDefault(S => S.ServiceType == "urn:schemas-upnp-org:service:ContentDirectory:1");

        if (serverEntry != null)
        {
            var controlUrl = $"http://{server.Host}:{server.Port}{serverEntry.ControlURL}";

            return new DlnaItem
            {
                IsBrowsable = true,
                IsServer = true,
                Name = data?.Device?.FriendlyName ?? string.Empty,
                Uri = new Uri(controlUrl),
                Id = string.Empty,
            };
        }
        throw new InvalidOperationException("Invalid response");
    }

    private string CreateBrowseXml(string id, int startIndex = 0, int requestedCount = 0)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(requestedCount, 0);

        var brwseObject = new BrowseRequest
        {
            ObjectID = id,
            BrowseFlag = "BrowseDirectChildren",
            Filter = "*",
            StartingIndex = startIndex,
            RequestedCount = requestedCount,
            SortCriteria = string.Empty,
        };

        var xml = _browseRequestSerializer.SerializeSoap(brwseObject, false);

        return xml;
    }
}
