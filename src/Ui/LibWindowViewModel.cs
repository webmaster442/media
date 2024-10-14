
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Database;
using Media.Interfaces;

namespace Media.Ui;

internal sealed partial class LibWindowViewModel : ObservableObject, IViewModel
{
    private readonly IUiFunctions _uiFunctions;
    private readonly MediaDbSerives _mediaDbSerives;
    private readonly Stack<string> _navigationHistory;

    [ObservableProperty]
    private object _contentItem;

    [ObservableProperty]
    private string _currentUrl;

    public LibWindowViewModel(IUiFunctions uiFunctions, MediaDbSerives mediaDbSerives)
    {
        _contentItem = new object();
        _uiFunctions = uiFunctions;
        _mediaDbSerives = mediaDbSerives;
        _currentUrl = string.Empty;
        _navigationHistory = new Stack<string>();
    }

    public void Initialize()
    {
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        if (_navigationHistory.Count > 0)
        {
            CurrentUrl = _navigationHistory.Pop();
            ContentItem = await GetViewModel(CurrentUrl);
        }
    }

    [RelayCommand]
    private async Task NavigateTo(string url)
    {
        _navigationHistory.Push(CurrentUrl);
        CurrentUrl = url;
        ContentItem = await GetViewModel(CurrentUrl);
    }

    public async Task<object> GetViewModel(string url)
    {
        if (string.IsNullOrEmpty(url))
            return "";

        string[] parts = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
        switch (parts[0])
        {
            case NavigationItem.ArtistPrefix:
                var artists = await _mediaDbSerives.GetArtists();
                return new LibNavigatableViewModel(artists.Select(artist => new NavigationItem
                {
                    Title = artist,
                    Url = $"{NavigationItem.ArtistPrefix}/{artist}"
                }));
            case NavigationItem.AlbumPrefix:
                var albums = await _mediaDbSerives.GetAlbums();
                return new LibNavigatableViewModel(albums.Select(album => new NavigationItem
                {
                    Title = album.Name,
                    Url = $"{NavigationItem.AlbumPrefix}/{album.Id}"
                }));
            case NavigationItem.YearPrefix:
                var years = await _mediaDbSerives.GetYears();
                return new LibNavigatableViewModel(years.Select(year => new NavigationItem
                {
                    Title = year.ToString(),
                    Url = $"{NavigationItem.AlbumPrefix}/{year}"
                }));
            case NavigationItem.GenrePrefix:
                var genres = await _mediaDbSerives.GetGenres();
                return new LibNavigatableViewModel(genres.Select(genre => new NavigationItem
                {
                    Title = genre,
                    Url = $"{NavigationItem.AlbumPrefix}/{genre}"
                }));
            default:
                return $"Don't know how to navigate to: {url}";
        }
    }
}
