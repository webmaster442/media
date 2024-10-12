// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Dto;
using Media.Infrastructure;
using Media.Interfaces;
using Media.Interop;

namespace Media.Ui;

internal partial class DropConvertViewModel : ObservableObject, IViewModel
{
    private readonly IUiFunctions _uiFunctions;
    private readonly FFMpeg _fFMpeg;

    [ObservableProperty]
    private string _selectedPath;

    partial void OnSelectedPathChanged(string? oldValue, string newValue)
        => SelectedPathDisplay = Path.GetFileName(newValue);

    [ObservableProperty]
    private string _selectedPathDisplay;

    [ObservableProperty]
    private Preset? _selectedPreset;

    public ObservableCollection<Preset> PresetCollection { get; }

    public DropConvertViewModel(IUiFunctions uiFunctions, ConfigAccessor configAccessor)
    {
        _uiFunctions = uiFunctions;
        _fFMpeg = new FFMpeg(configAccessor);
        _selectedPath = Environment.CurrentDirectory;
        _selectedPathDisplay = Path.GetFileName(_selectedPath);
        PresetCollection = new ObservableCollection<Preset>(); 
    }

    public async void Initialize()
    {
        var presets = await Presets.LoadPresetArray();
        foreach (var preset in presets)
        {
            PresetCollection.Add(preset);
        }
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

        if (file.Contains('%'))
        {
            //cmd.exe fix
            file = file.Replace("%", "%%");
        }
        var outputFile = Path.Combine(SelectedPath, Path.ChangeExtension(Path.GetFileName(file), SelectedPreset.Extension));
        var cmdLine = SelectedPreset.CommandLine.Replace(Preset.InputPlaceHolder, file);
        return _fFMpeg.GetCommandText(cmdLine.Replace(Preset.OutputPlaceHolder, outputFile));
    }

    public void HandleDrop(string[] files)
    {
        if (SelectedPreset == null)
        {
            _uiFunctions.ErrorMessage("Please select a preset first", "No preset selected");
            return;
        }

        List<string> skipped = new();

        var scriptFile = Path.Combine(SelectedPath, Path.ChangeExtension(Path.GetFileName(SelectedPath), ".cmd"));
        var builder = new PowershellBuilder();
        builder.WithUtf8Enabled();
        builder.WithWindowTitle(Path.GetFileNameWithoutExtension(SelectedPath));

        foreach (var file in files)
        {
            if (File.Exists(file)
                && _fFMpeg.SupportedFormats.Contains(Path.GetExtension(file)))
            {
                string cmdLine = CreateCommandLine(file);
                builder.WithCommandLine(cmdLine);
            }
            else
            {
                skipped.Add(Path.GetFileName(file));
            }
        }

        File.WriteAllText(scriptFile, builder.Build());

        if (skipped.Count > 0)
        {
            _uiFunctions.WarningMessage($"Skipped files:\r\n{string.Join("\r\n", skipped)}", "Skipped files");
        }

        bool shouldRun = _uiFunctions.QuestionMessage("Do you want to run the generated script?", "Run script?");
        if (shouldRun)
        {
            var powerShell = new Powershell();
            powerShell.RunScript(scriptFile);
        }

    }
}
