using Media.Database;
using Media.Dto.Radio;

namespace Media.Infrastructure;

internal class RadioStationsClient : ApiClient
{
    private readonly ApiCacheAdapter _cacheAdapter;

    public RadioStationsClient()
    {
        _cacheAdapter = new ApiCacheAdapter();
    }

    public async Task<IReadOnlyList<Country>> GetRadioStationCountries()
    {
        if (_cacheAdapter.RadioCountriesLastFetch.IsYoungerThan(TimeSpan.FromDays(7))
            && _cacheAdapter.Countries.Count > 0)
        {
            return (IReadOnlyList<Country>)_cacheAdapter.Countries;
        }

        string url = $"{ApiUrls.RadioBrowserApi}/countries";
        using var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        List<Country>? deserialized = JsonSerializer.Deserialize<List<Country>>(json);

        if (deserialized is not null)
        {
            _cacheAdapter.Countries.Clear();
            _cacheAdapter.Countries.AddRange(deserialized);
            await _cacheAdapter.Save();
            return deserialized;
        }

        throw new InvalidOperationException("Data deserialize failed");
    }

    public async Task<IReadOnlyList<Station>> GetRadioStations(string countryCode)
    {
        if (_cacheAdapter.RadioStationsLastFetch.IsYoungerThan(TimeSpan.FromDays(7))
            && _cacheAdapter.Stations.ContainsKey(countryCode))
        {
            return _cacheAdapter.Stations[countryCode];
        }

        string url = $"{ApiUrls.RadioBrowserApi}/stations/bycountry/{countryCode}";
        using var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var deserialized = JsonSerializer.Deserialize<List<Station>>(json);

        if (deserialized is not null)
        {
            _cacheAdapter.Stations[countryCode] = deserialized;
            await _cacheAdapter.Save();
            return deserialized;
        }
        
        throw new InvalidOperationException("Data deserialize failed");
    }
}
