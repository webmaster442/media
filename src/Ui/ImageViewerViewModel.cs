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
    private int _currentImageIndex;

    public ObservableRangeCollection<string> ImageFiles { get; }

    [ObservableProperty]
    private string _currentImage;

    public ImageViewerViewModel(string folder)
    {
        _currentImageIndex = 0;
        CurrentImage = string.Empty;
        ImageFiles = new ObservableRangeCollection<string>();
        _folder = folder;
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
    }

    private bool CanNextExecute()
        => ImageFiles.Count > 0 && ((_currentImageIndex + 1) < ImageFiles.Count);

    [RelayCommand]
    private void Next()
    {
        if ((_currentImageIndex + 1) < ImageFiles.Count)
        {
            _currentImageIndex++;
            CurrentImage = ImageFiles[_currentImageIndex];
        }
    }

    private bool CanPreviousExecute()
        => (ImageFiles.Count > 0) && (_currentImageIndex >= 1);

    [RelayCommand(CanExecute = nameof(CanPreviousExecute))]
    private void Previous()
    {
        if (_currentImageIndex >= 1)
        {
            _currentImageIndex--;
            CurrentImage = ImageFiles[_currentImageIndex];
        }
    }
}
