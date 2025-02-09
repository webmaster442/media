// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Interfaces;
using Media.Interop;
using Media.Ui.Controls;

using Microsoft.Extensions.Logging;

namespace Media.Ui;

internal sealed partial class ImageViewerViewModel : ObservableObject, IViewModel
{
    private readonly string _folder;
    private readonly IUiFunctions _uiFunctions;

    private int _currentImageIndex;

    public ObservableRangeCollection<string> ImageFiles { get; }

    [ObservableProperty]
    public partial string CurrentImage { get; set; }

    public ILogger Logger { get; }

    partial void OnCurrentImageChanged(string value)
    {
        if (ImageFiles.Count < 1) return;
        _currentImageIndex = ImageFiles.IndexOf(value);
        double progress = (double)_currentImageIndex / ImageFiles.Count;
        _uiFunctions.Report(progress);
    }

    [ObservableProperty]
    public partial string WindowTitle { get; set; }

    public ImageViewerViewModel(string folder, IUiFunctions uiFunctions, ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger<ImageViewerViewModel>();
        _folder = folder;
        _uiFunctions = uiFunctions;
        WindowTitle = $"Image Viewer - {Path.GetFileName(_folder)}";
        _currentImageIndex = 0;
        ImageFiles = new ObservableRangeCollection<string>();
        CurrentImage = string.Empty;
    }

    public void Initialize()
    {
        var files = Directory.EnumerateFiles(_folder)
            .Where(file => FileRecognizer.GetFileType(file) == FileRecognizer.FileType.Image);

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
