// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.DbAdapters;
using Media.Dto;
using Media.Infrastructure;
using Media.Interfaces;
using Media.Interop;

using Microsoft.Extensions.Logging;

namespace Media.Ui;

internal sealed partial class DropConvertViewModel : ObservableObject, IViewModel
{
    private readonly IUiFunctions _uiFunctions;
    private readonly FFMpeg _fFMpeg;

    [ObservableProperty]
    public partial string SelectedPath { get; set; }

    partial void OnSelectedPathChanged(string oldValue, string newValue)
        => SelectedPathDisplay = Path.GetFileName(newValue);

    [ObservableProperty]
    public partial string SelectedPathDisplay { get; set; }

    [ObservableProperty]
    public partial Preset? SelectedPreset { get; set; }

    public ObservableCollection<Preset> PresetCollection { get; }

    public ILogger Logger { get; }

    public DropConvertViewModel(IUiFunctions uiFunctions, ConfigAdapter configAccessor, ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger<DropConvertViewModel>();
        _uiFunctions = uiFunctions;
        _fFMpeg = new FFMpeg(configAccessor);
        SelectedPath = Environment.CurrentDirectory;
        SelectedPathDisplay = Path.GetFileName(SelectedPath);
        PresetCollection = new ObservableCollection<Preset>();
    }

    public async void Initialize()
    {
        var presets = await Presets.LoadPresetArray();
        foreach (var preset in presets.OrderBy(x => x.Category).ThenBy(x => x.Name))
        {
            PresetCollection.Add(preset);
        }
        SelectedPreset = PresetCollection[0];
        _fFMpeg.TryGetInstalledPath(out string? path);
        if (string.IsNullOrEmpty(path))
        {
            _uiFunctions.ErrorMessage("FFMpeg not found. Please install it first with update ffmpeg", "FFMpeg not found");
            _uiFunctions.Exit(ExitCodes.Error);
        }
    }

    [RelayCommand]
    private void Browse()
    {
        string? selection = _uiFunctions.SelectFolderDialog(SelectedPath);
        if (!string.IsNullOrEmpty(selection))
        {
            SelectedPath = selection;
        }
    }

    private string CreateCommandLine(string file)
    {
        if (SelectedPreset == null)
        {
            throw new InvalidOperationException("No preset selected");
        }

        var outputFile = Path.Combine(SelectedPath, Path.ChangeExtension(Path.GetFileName(file), SelectedPreset.Extension));
        var args = SelectedPreset.GetCommandLine(file, outputFile);
        return _fFMpeg.GetCommandText(args);
    }

    public void HandleDrop(string[] files)
    {
        if (SelectedPreset == null)
        {
            _uiFunctions.ErrorMessage("Please select a preset first", "No preset selected");
            return;
        }

        List<string> skipped = new();

        var scriptFile = Path.Combine(SelectedPath, Path.ChangeExtension(Path.GetFileName(SelectedPath), ".ps1"));
        var builder = new PowershellBuilder()
            .WithUtf8Enabled()
            .WithWindowTitle(Path.GetFileNameWithoutExtension(SelectedPath))
            .WithClear();

        foreach (var file in files)
        {
            if (File.Exists(file)
                && FileRecognizer.IsDropConvertSupported(file))
            {
                builder.WithCommand(CreateCommandLine(file));
            }
            else
            {
                skipped.Add(Path.GetFileName(file));
            }
        }

        builder.WithMessage("Finished");

        File.WriteAllText(scriptFile, builder.Build());

        if (skipped.Count > 0)
        {
            _uiFunctions.WarningMessage($"Skipped files:\r\n{string.Join("\r\n", skipped)}", "Skipped files");
        }

        bool shouldRun = _uiFunctions.QuestionMessage("Do you want to run the generated script?", "Run script?");
        if (shouldRun)
        {
            var powerShell = new Powershell(updatePathVar: false);
            powerShell.RunScript(scriptFile);
        }

    }
}
