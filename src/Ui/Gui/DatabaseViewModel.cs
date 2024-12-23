using System.Collections;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.DbAdapters;
using Media.Infrastructure;

namespace Media.Ui.Gui;

internal partial class DatabaseViewModel : ObservableObject
{
    private readonly GuiDatabaseAdapter _guiDatabaseAdapter;

    [ObservableProperty]
    public partial IEnumerable Results { get; set; }

    public DatabaseViewModel(GuiDatabaseAdapter guiDatabaseAdapter)
    {
        _guiDatabaseAdapter = guiDatabaseAdapter;
        Results = Enumerable.Empty<object>();
    }

    [RelayCommand]
    private async Task GetFromRange(string rangeParam)
    {
        (DateTime start, DateTime end) range = (DateTime.MinValue, DateTime.MaxValue);
        switch (rangeParam)
        {
            case "today":
                range = DateTime.Now.Day();
                break;
            case "last3days":
                range = DateTime.Now.Last3Days();
                break;
            case "week":
                range = DateTime.Now.Week();
                break;
            case "month":
                range = DateTime.Now.Month();
                break;
            case "all":
                range = (DateTime.MinValue, DateTime.MaxValue);
                break;
        }
        Results = await _guiDatabaseAdapter.GetPlayedEntries(range.start, range.end);
    }
}
