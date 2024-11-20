// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Radio;

namespace Media.Database;

internal class ApiCacheAdapter : DocumentStoreAdapter
{
    private const string KeyRadioCountries = "radiocountries";
    private const string KeyRadioStations = "radiostations";

    public ApiCacheAdapter()
    {
        Countries = new DbList<Country>();
        Stations = new DbDictionary<string, List<Station>>();
    }

    public DateTime RadioStationsLastFetch { get; set; }
    public DateTime RadioCountriesLastFetch { get; set; }

    public override async Task Init()
    {
        RadioCountriesLastFetch = await _store.GetCollectionLastModification(KeyRadioCountries);
        RadioStationsLastFetch = await _store.GetCollectionLastModification(KeyRadioStations);
        Countries = await _store.DeserializeCollectionAsList<Country>(KeyRadioCountries);
        Stations = await _store.DeserializeCollectionAsDictionary<string, List<Station>>(KeyRadioStations);
    }

    public DbList<Country> Countries { get; private set; }

    public DbDictionary<string, List<Station>> Stations { get; private set; }

    public override async Task Save()
    {
        if (Countries.IsDirty)
        {
            await _store.SerializeCollection(KeyRadioCountries, Countries);
            RadioCountriesLastFetch = DateTime.UtcNow;
        }

        if (Stations.IsDirty)
        {
            await _store.SerializeCollection(KeyRadioStations, Stations);
            RadioStationsLastFetch = DateTime.UtcNow;
        }
    }
}
