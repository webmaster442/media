// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Interfaces;
using Media.Ui.Controls;

namespace Media.Ui;

internal sealed partial class ImageViewerViewModel : ObservableObject, IViewModel
{
    private readonly string _folder;
    private readonly IUiFunctions _uiFunctions;

    private int _currentImageIndex;

    public ObservableRangeCollection<string> ImageFiles { get; }

    [ObservableProperty]
    private string _currentImage;

    partial void OnCurrentImageChanged(string value)
    {
        if (ImageFiles.Count < 1) return;
        _currentImageIndex = ImageFiles.IndexOf(value);
        double progress = (double)_currentImageIndex / ImageFiles.Count;
        _uiFunctions.Report(progress);
    }

    [ObservableProperty]
    private string _windowTitle;

    public ImageViewerViewModel(string folder, IUiFunctions uiFunctions)
    {
        _folder = folder;
        _uiFunctions = uiFunctions;
        _windowTitle = $"Image Viewer - {Path.GetFileName(_folder)}";
        _currentImageIndex = 0;
        ImageFiles = new ObservableRangeCollection<string>();
        CurrentImage = string.Empty;
    }

    private static bool IsImageFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        return extension is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".webp";
    }

    public void Initialize()
    {
        var files = Directory.EnumerateFiles(_folder)
            .Where(file => IsImageFile(file));

        ImageFiles.AddRange(files);
        
        if (ImageFiles.Count > 0)
            CurrentImage = ImageFiles[0];

        NextCommand.NotifyCanExecuteChanged();
    }

    private bool CanNextExecute()
        => ImageFiles.Count > 0 && ((_currentImageIndex + 1) < ImageFiles.Count);

    [RelayCommand(CanExecute = nameof(CanNextExecute))]
    private void Next()
    {
        if ((_currentImageIndex + 1) < ImageFiles.Count)
        {
            _currentImageIndex++;
            CurrentImage = ImageFiles[_currentImageIndex];
        }
        PreviousCommand.NotifyCanExecuteChanged();
        NextCommand.NotifyCanExecuteChanged();
    }

    private bool CanPreviousExecute()
        => (ImageFiles.Count > 0) && ((_currentImageIndex - 1) >= 0);

    [RelayCommand(CanExecute = nameof(CanPreviousExecute))]
    private void Previous()
    {
        if (_currentImageIndex >= 1)
        {
            _currentImageIndex--;
            CurrentImage = ImageFiles[_currentImageIndex];
        }
        PreviousCommand.NotifyCanExecuteChanged();
        NextCommand.NotifyCanExecuteChanged();
    }
}
