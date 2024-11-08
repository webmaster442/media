using Media.Dto.Radio;

namespace Media.Infrastructure;

internal class RadioStationsClient : ApiClient
{
    public async Task<Country[]> GetRadioStationCountries()
    {
        string url = $"{ApiUrls.RadioBrowserApi}/countries";
        using var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var deserialized = JsonSerializer.Deserialize<Country[]>(json);

        return deserialized ?? throw new InvalidOperationException("Data deserialize failed");
    }

    public async Task<Station[]> GetRadioStationsByCountry(string countryCode)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(countryCode);

        string url = $"{ApiUrls.RadioBrowserApi}/stations/bycountry/{countryCode}";
        using var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var deserialized = JsonSerializer.Deserialize<Station[]>(json);

        return deserialized ?? throw new InvalidOperationException("Data deserialize failed");
    }
}
