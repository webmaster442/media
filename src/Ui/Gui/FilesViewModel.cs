// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Media.DbAdapters;
using Media.Interfaces;
using Media.Interop;
using Media.Ui.Controls;
using Media.Ui.Converters;

using Microsoft.Extensions.Logging;

namespace Media.Ui.Gui;
internal partial class FilesViewModel : ObservableObject, IViewModel
{
    private readonly IUiFunctions _uiFunctions;
    private readonly ConfigAdapter _configAdapter;
    private readonly GuiDatabaseAdapter _guiDatabaseAdapter;

    [ObservableProperty]
    public partial string CurrentPath { get; set; }

    partial void OnCurrentPathChanged(string value)
        => AddBookMarkCommand.NotifyCanExecuteChanged();

    public ObservableRangeCollection<DriveModel> Drives { get; }
    public ObservableRangeCollection<PathPartModel> PathParts { get; }
    public ObservableRangeCollection<FolderItem> Items { get; }
    public ObservableRangeCollection<BookmarkViewModel> Bookmarks { get; }

    [ObservableProperty]
    public partial bool ShowHidden { get; set; }

    [ObservableProperty]
    public partial FolderItem? SelectedItem { get; set; }

    public ILogger Logger { get; }

    partial void OnShowHiddenChanged(bool value)
    {
        Navigate(CurrentPath);
    }

    public FilesViewModel(IUiFunctions uiFunctions,
                          ConfigAdapter configAdapter,
                          GuiDatabaseAdapter guiDatabaseAdapter,
                          ILoggerFactory loggerFactory)
    {
        SelectedItem = null;
        CurrentPath = string.Empty;
        Drives = new ObservableRangeCollection<DriveModel>();
        PathParts = new ObservableRangeCollection<PathPartModel>();
        Items = new ObservableRangeCollection<FolderItem>();
        Bookmarks = new ObservableRangeCollection<BookmarkViewModel>();
        Logger = loggerFactory.CreateLogger<FilesViewModel>();
        _uiFunctions = uiFunctions;
        _configAdapter = configAdapter;
        _guiDatabaseAdapter = guiDatabaseAdapter;
    }

    public async void Initialize()
    {
        Bookmarks.Clear();
        var bookmarks = await _guiDatabaseAdapter.GetBookmarks();
        Bookmarks.AddRange(bookmarks);
        RefreshDriveList();
    }

    [RelayCommand]
    private void RefreshDriveList()
    {
        var drives = DriveInfo.GetDrives();
        var models = new List<DriveModel>(drives.Length);
        foreach (var drive in drives)
        {
            try
            {
                if (!drive.IsReady) continue;

                models.Add(new DriveModel
                {
                    DriveType = drive.DriveType,
                    Label = drive.VolumeLabel,
                    Letter = drive.Name,
                    PecentFull = 1.0 - ((double)drive.TotalFreeSpace / drive.TotalSize),
                    FreeSpace = FileSizeConverter.Convert(drive.TotalFreeSpace),
                    TotalSize = FileSizeConverter.Convert(drive.TotalSize)
                });
            }
            catch (Exception e)
            {
                _uiFunctions.ErrorMessage("Error", e.Message);
            }
        }
        Drives.Clear();
        Drives.AddRange(models);
    }

    [RelayCommand]
    private void DriveSelect(DriveModel drive)
    {
        Navigate(drive.Letter);
    }

    private bool CanStartShell(string path)
        => Directory.Exists(path);

    [RelayCommand(CanExecute = nameof(CanStartShell))]
    private void SartShell(string path)
        => SelfInterop.StartShell(path);

    [RelayCommand]
    private void Navigate(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (!path.EndsWith('\\'))
            path += '\\';

        CurrentPath = path;
        PathParts.Clear();
        PathParts.AddRange(CreatePathParts(path));

        var items = new List<FolderItem>();
        try
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (var subdir in dir.GetDirectories())
            {
                if (subdir.Attributes.HasFlag(FileAttributes.Hidden) && !ShowHidden) continue;

                items.Add(new FolderItem
                {
                    FullPath = subdir.FullName,
                    IsDirectory = true,
                    LastModified = subdir.LastWriteTime,
                    Size = string.Empty,
                    FileType = Interop.FileRecognizer.FileType.Other,
                    Extension = "Directory"
                });
            }

            foreach (var file in dir.GetFiles())
            {
                if (file.Attributes.HasFlag(FileAttributes.Hidden) && !ShowHidden) continue;

                items.Add(new FolderItem
                {
                    FullPath = file.FullName,
                    IsDirectory = false,
                    LastModified = file.LastWriteTime,
                    Size = FileSizeConverter.Convert(file.Length),
                    FileType = Interop.FileRecognizer.GetFileType(file.FullName),
                    Extension = file.Extension
                });
            }
            Items.Clear();
            Items.AddRange(items);
        }
        catch (Exception e)
        {
            _uiFunctions.ErrorMessage("Error", e.Message);
        }
    }

    private bool CanAddbookMark()
        => Path.Exists(CurrentPath);

    [RelayCommand(CanExecute = nameof(CanAddbookMark))]
    private async Task AddBookMark()
    {
        if (string.IsNullOrEmpty(CurrentPath))
            return;

        var model = new BookmarkViewModel
        {
            Name = Path.GetFileName(Path.GetDirectoryName(CurrentPath)) ?? CurrentPath,
            Path = CurrentPath
        };

        await _guiDatabaseAdapter.AddBookMark(model);
        Bookmarks.Add(model);
    }

    [RelayCommand]
    private async Task RemoveBookmark(BookmarkViewModel bookmark)
    {
        await _guiDatabaseAdapter.RemoveBookmark(bookmark);
        Bookmarks.Remove(bookmark);
    }

    [RelayCommand]
    private void DoubleClick(FolderItem item)
    {
        if (item == null)
            return;

        if (item.IsDirectory)
        {
            Navigate(item.FullPath);
        }
        else if (item.FileType.IsMpvSupportedType())
        {
            SelfInterop.Play(item.FullPath);
        }
        else
        {
            Interop.Windows.ShellExecute(item.FullPath);
        }
    }

    private bool CanPlay(FolderItem item)
        => item?.FileType.IsMpvSupportedType() == true;

    [RelayCommand(CanExecute = nameof(CanPlay))]
    private void Play(FolderItem item)
    {
        if (item.FileType.IsMpvSupportedType())
        {
            SelfInterop.Play(item.FullPath);
        }
    }

    [RelayCommand(CanExecute = nameof(CanPlay))]
    private void SendToPlaylist(FolderItem item)
    {
        if (item.FileType.IsMpvSupportedType())
        {
            WeakReferenceMessenger.Default.Send(new PlaylistViewModel.AddToPlaylistMessage
            {
                FullPath = item.FullPath
            });
        }
    }

    [RelayCommand(CanExecute = nameof(CanPlay))]
    private async Task Preview(FolderItem item)
    {
        if (item == null)
            return;

        if (item.FileType == FileRecognizer.FileType.Video)
        {
            _uiFunctions.BeginAsyncOperation();
            var view = new PreviewViewModel(_configAdapter, item.FullPath);
            await view.Initialize();
            _uiFunctions.EndAsyncOperation();
            _uiFunctions.ShowInternalWindow(item.FullPath, view);
        }
    }

    private IEnumerable<PathPartModel> CreatePathParts(string path)
    {
        var parts = path.Split('\\', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<PathPartModel>(parts.Length);
        var currentPath = string.Empty;
        foreach (var part in parts)
        {
            currentPath = Path.Combine(currentPath, part);
            result.Add(new PathPartModel
            {
                DisplayName = $"{part}\\",
                FullPath = currentPath
            });
        }
        return result;
    }
}
