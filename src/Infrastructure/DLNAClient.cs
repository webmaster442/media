// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Web;
using System.Xml.Serialization;

using Media.Dto.Dlna;
using Media.Dto.Internals;

namespace Media.Infrastructure;

internal sealed class DLNAClient : IDisposable
{
    private readonly HttpClient _client;
    private readonly XmlSerializer _rootSerializer;
    private readonly XmlSerializer _envelopeSerializer;

    public DLNAClient()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
        _rootSerializer = new XmlSerializer(typeof(Root));
        _envelopeSerializer = new XmlSerializer(typeof(Envelope));
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    private static Dictionary<string, string> ParseSSDPResponse(string str)
    {
        string[] lines = str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, string> results = new();
        foreach (var line in lines)
        {
            int colonIndex = line.IndexOf(':');
            if (colonIndex > 0)
            {
                string key = line[..colonIndex];
                string value = line[(colonIndex + 1)..];
                results[key] = value;
            }
        }
        return results;
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

                        var ssdpResponse = ParseSSDPResponse(responseText);

                        if (ssdpResponse["ST"].Contains("urn:schemas-upnp-org:device:MediaServer"))
                        {
                            var url = new Uri(ssdpResponse["LOCATION"]);
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
        using var reader = new StringReader(HttpUtility.HtmlDecode(xml));
        if (_envelopeSerializer.Deserialize(reader) is Envelope envelope
            && envelope?.Body?.BrowseResponse?.Result?.DIDLLite?.Items?.Length > 0)
        {
            List<DlnaItem> items = new();
            foreach (var objItem in envelope.Body.BrowseResponse.Result.DIDLLite.Items)
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
        using var reader = new StringReader(content);
        var data = _rootSerializer.Deserialize(reader) as Root;
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

    private static string CreateBrowseXml(string id)
    {
        return $"""
            <?xml version=\"1.0\"?>
            <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/" s:encodingStyle="http://schemas.xmlsoap.org/soap/encoding/">
                <s:Body>
                    <u:Browse xmlns:u="urn:schemas-upnp-org:service:ContentDirectory:1">
                        <ObjectID>{id}</ObjectID>
                        <BrowseFlag>BrowseDirectChildren</BrowseFlag>
                        <Filter>*</Filter>
                        <StartingIndex>0</StartingIndex>
                        <RequestedCount>1000</RequestedCount>
                        <SortCriteria></SortCriteria>
                    </u:Browse>
                </s:Body>
            </s:Envelope>
            """;
    }
}
