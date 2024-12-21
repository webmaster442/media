// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;
using Media.Dto.Radio;

namespace Media.Infrastructure;

internal class RadioStationsClient : ApiClient
{
    private readonly ApiCacheAdapter _cacheAdapter;

    public RadioStationsClient(ApiCacheAdapter apiCacheAdapter)
    {
        _cacheAdapter = apiCacheAdapter;
    }

    public async Task<IReadOnlyList<Country>> GetRadioStationCountries()
    {
        var cacheEntry = await _cacheAdapter.GetEntry(ApiCacheAdapter.RadioCountries);
        if (cacheEntry != null
            && cacheEntry.IsValid(DateTime.Now))
        {
            return cacheEntry.Deserialize<List<Country>>();
        }

        string url = $"{ApiUrls.RadioBrowserApi}/countries";
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

        string url = $"{ApiUrls.RadioBrowserApi}/stations/bycountry/{countryCode}";
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
