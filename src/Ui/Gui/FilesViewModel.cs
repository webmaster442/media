using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Media.Interfaces;
using Media.Interop;
using Media.Ui.Controls;
using Media.Ui.Converters;

namespace Media.Ui.Gui;
internal partial class FilesViewModel : ObservableObject
{
    private readonly IUiFunctions _uiFunctions;
    private string _currentPath;

    public ObservableRangeCollection<DriveModel> Drives { get; }
    public ObservableRangeCollection<PathPartModel> PathParts { get; }
    public ObservableRangeCollection<FolderItem> Items { get; }

    [ObservableProperty]
    private bool _showHidden;

    partial void OnShowHiddenChanged(bool value)
    {
        Navigate(_currentPath);
    }

    public FilesViewModel(IUiFunctions uiFunctions)
    {
        _currentPath = string.Empty;
        Drives = new ObservableRangeCollection<DriveModel>();
        PathParts = new ObservableRangeCollection<PathPartModel>();
        Items = new ObservableRangeCollection<FolderItem>();
        _uiFunctions = uiFunctions;
    }

    [RelayCommand]
    public void RefreshDriveList()
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

    [RelayCommand]
    private void Navigate(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (!path.EndsWith('\\'))
            path += '\\';

        _currentPath = path;
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

    [RelayCommand]
    private void DoubleClick(FolderItem item)
    {
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
            Windows.ShellExecute(item.FullPath);
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
