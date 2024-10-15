using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using Media.Dto.MediaDb;

namespace Media.Ui;

internal sealed class LibDetailsViewModel : ObservableObject
{
    public ObservableCollection<MusicFile> MusicFiles { get; }

    public LibDetailsViewModel(IEnumerable<MusicFile> musicFiles)
    {
        MusicFiles = new ObservableCollection<MusicFile>(musicFiles);
    }
}