// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Net.NetworkInformation;
using System.Net;

using Media.DbAdapters;
using Media.Dto.Radio;

namespace Media.Infrastructure;

internal class RadioStationsClient : ApiClient
{
    private readonly ApiCacheAdapter _cacheAdapter;

    public RadioStationsClient(ApiCacheAdapter apiCacheAdapter)
    {
        _cacheAdapter = apiCacheAdapter;
    }

    private static string GetRadioBrowserApiUrl()
    {
        // Get fastest ip of dns
        var ips = Dns.GetHostAddresses(ApiUrls.RadioBrowserApiHost);
        long lastRoundTripTime = long.MaxValue;
        string searchUrl = ApiUrls.RadioBrowserFallbackUrl; // Fallback
        foreach (IPAddress ipAddress in ips)
        {
            using var ping = new Ping();
            var reply = ping.Send(ipAddress);
            if (reply != null &&  reply.RoundtripTime < lastRoundTripTime)
            {
                lastRoundTripTime = reply.RoundtripTime;
                searchUrl = ipAddress.ToString();
            }
        }

        // Get clean name
        IPHostEntry hostEntry = Dns.GetHostEntry(searchUrl);
        if (!string.IsNullOrEmpty(hostEntry.HostName))
        {
            searchUrl = hostEntry.HostName;
        }

        return searchUrl;
    }

    public async Task<IReadOnlyList<Country>> GetRadioStationCountries()
    {
        var cacheEntry = await _cacheAdapter.GetEntry(ApiCacheAdapter.RadioCountries);
        if (cacheEntry != null
            && cacheEntry.IsValid(DateTime.Now))
        {
            return cacheEntry.Deserialize<List<Country>>();
        }

        string url = $"http://{GetRadioBrowserApiUrl()}/json/countries";
        using var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        List<Country>? deserialized = JsonSerializer.Deserialize<List<Country>>(json);

        if (deserialized is not null)
        {
            await _cacheAdapter.SetEntry(ApiCacheAdapter.RadioCountries, json, DateTime.Now);
            return deserialized;
        }

        throw new InvalidOperationException("Data deserialize failed");
    }

    public async Task<IReadOnlyList<Station>> GetRadioStations(string countryCode)
    {
        var key = $"{ApiCacheAdapter.StationBase}{countryCode}";

        var cacheEntry = await _cacheAdapter.GetEntry(key);

        if (cacheEntry != null
            && cacheEntry.IsValid(DateTime.Now))
        {
            return cacheEntry.Deserialize<List<Station>>();
        }

        string url = $"http://{GetRadioBrowserApiUrl()}/json/stations/bycountry/{countryCode}";
        using var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var deserialized = JsonSerializer.Deserialize<List<Station>>(json);

        if (deserialized is not null)
        {
            await _cacheAdapter.SetEntry(key, json, DateTime.Now);
            return deserialized;
        }
        
        throw new InvalidOperationException("Data deserialize failed");
    }
}
