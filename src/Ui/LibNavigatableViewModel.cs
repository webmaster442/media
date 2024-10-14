using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using Media.Database;
using Media.Dto.MediaDb;

namespace Media.Ui;

public sealed class NavigationItem
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public const string AlbumPrefix = "album";
    public const string ArtistPrefix = "artist";
    public const string GenrePrefix = "genre";
    public const string YearPrefix = "year";

}

internal sealed class LibNavigatableViewModel : ObservableObject
{
    public ObservableCollection<NavigationItem> VisibleItems { get; }

    public LibNavigatableViewModel(IEnumerable<NavigationItem> items)
    {
        VisibleItems = new ObservableCollection<NavigationItem>(items);
    }
}

internal sealed class LibDetailsViewModel : ObservableObject
{
    public ObservableCollection<MusicFile> MusicFiles { get; }

    public LibDetailsViewModel(IEnumerable<MusicFile> musicFiles)
    {
        MusicFiles = new ObservableCollection<MusicFile>(musicFiles);
    }
}