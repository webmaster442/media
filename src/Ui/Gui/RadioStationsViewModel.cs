using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Dto.Radio;
using Media.Infrastructure;
using Media.Interfaces;
using Media.Ui.Controls;

namespace Media.Ui.Gui;

internal sealed partial class RadioStationsViewModel : ObservableObject, IViewModel
{
    private readonly RadioStationsClient _radioStationsClient;
    private readonly IUiFunctions _uiFunctions;

    public ObservableRangeCollection<Country> Countries { get; }
    public ObservableRangeCollection<Station> Stations { get; }

    public RadioStationsViewModel(RadioStationsClient radioStationsClient, IUiFunctions uiFunctions)
    {
        Countries = new ObservableRangeCollection<Country>();
        Stations = new ObservableRangeCollection<Station>();
        _radioStationsClient = radioStationsClient;
        _uiFunctions = uiFunctions;
    }

    public async void Initialize()
    {
        _uiFunctions.BlockUi();
        var countries = await _radioStationsClient.GetRadioStationCountries();
        Countries.Clear();
        Countries.AddRange(countries);
        _uiFunctions.UnblockUi();
    }

    [RelayCommand]
    private async Task LoadCountry(Country selection)
    {
        _uiFunctions.BlockUi();
        var stations = await _radioStationsClient.GetRadioStationsByCountry(selection.Iso31661);
        Stations.Clear();
        Stations.AddRange(stations);
        _uiFunctions.UnblockUi();
    }
}
