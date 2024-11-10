using Media.Dto.Radio;

namespace Media.Database;

internal class ApiCacheAdapter : DocumentStoreAdapter
{
    private const string KeyRadioCountries = "radiocountries";
    private const string KeyRadioStations = "radiostations";

    public ApiCacheAdapter()
    {
        Countries = new DbList<Country>();
        Stations = new DbList<Station>();
    }

    public DateTime RadioStationsLastFetch { get; set; }
    public DateTime RadioCountriesLastFetch { get; set; }

    public override async Task Init()
    {
        RadioCountriesLastFetch = await _store.GetCollectionLastModification(KeyRadioCountries);
        RadioStationsLastFetch = await _store.GetCollectionLastModification(KeyRadioStations);
        Countries = await _store.DeserializeCollectionAsList<Country>(KeyRadioCountries);
        Stations = await _store.DeserializeCollectionAsList<Station>(KeyRadioStations);
    }

    public DbList<Country> Countries { get; private set; }

    public DbList<Station> Stations { get; private set; }

    public override async Task Save()
    {
        if (Countries.IsDirty)
            await _store.SerializeCollection(KeyRadioCountries, Countries);

        if (Stations.IsDirty)
            await _store.SerializeCollection(KeyRadioStations, Stations);
    }
}
