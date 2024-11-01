// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Interfaces;
using Media.Ui.Controls;

namespace Media.Ui;

internal partial class GuiViewModel : ObservableObject, IViewModel
{
    [ObservableProperty]
    private GuiCommand? _selectedCommand;

    [ObservableProperty]
    private bool _allwaysOnTop;

    partial void OnAllwaysOnTopChanged(bool value)
        => _configAccessor.SetAlwaysOnTop(value);

    [ObservableProperty]
    private bool _exitOnLauch;

    partial void OnExitOnLauchChanged(bool value)
        => _configAccessor.SetExitOnLaunch(value);

    private readonly IUiFunctions _uiFunctions;
    private readonly ConfigAccessor _configAccessor;

    public ObservableRangeCollection<GuiCommand> Commands { get; }

    public ObservableDictionary<string, string> Parameters { get; }

    public GuiViewModel(IUiFunctions uiFunctions, ConfigAccessor configAccessor)
    {
        Commands = new ObservableRangeCollection<GuiCommand>();
        Parameters = new ObservableDictionary<string, string>();
        _uiFunctions = uiFunctions;
        _configAccessor = configAccessor;
    }

    public void Initialize()
    {
        AllwaysOnTop = _configAccessor.GetAlwaysOnTop();
        ExitOnLauch = _configAccessor.GetExitOnLaunch();
        Commands.AddRange(GuiCommands.Commands.OrderBy(x => x.ButtonText));
        SelectedCommand = Commands[0];
    }

    [RelayCommand]
    private void Launch()
    {
        if (SelectedCommand == null)
        {
            _uiFunctions.WarningMessage("No command selected", "No command");
            return;
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule?.FileName ?? throw new UnreachableException(),
                Arguments = FillCommandLine()
            }
        };
        process.Start();
        if (ExitOnLauch)
            _uiFunctions.Exit(0);
    }

    private string FillCommandLine()
    {
        var commandLine = SelectedCommand!.CommandLine;
        foreach (var param in Parameters)
        {
            var replacekey = $"{{{param.Key}}}";
            commandLine = commandLine.Replace(replacekey, $"\"{param.Value}\"");
        }
        return commandLine;
    }
}
