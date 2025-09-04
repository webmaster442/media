// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Reflection;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Infrastructure.CommandAttributes;
using Media.Interfaces;
using Media.Interop;

using Microsoft.Extensions.Logging;

namespace Media.Ui;

internal sealed partial class InstallWindowViewModel : ObservableObject, IViewModel
{
    private readonly IUiFunctions _uiFunctions;

    public ILogger Logger { get; }

    [ObservableProperty]
    public partial bool CreateStartMenuIcons { get; set; }

    [ObservableProperty]
    public partial bool CreateDesktopIcons { get; set; }

    [ObservableProperty]
    public partial bool AddToPath { get; set; }

    [ObservableProperty]
    public partial bool UpdateDependencies { get; set; }

    public InstallWindowViewModel(IUiFunctions uiFunctions, ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger<InstallWindowViewModel>();
        _uiFunctions = uiFunctions;
        CreateStartMenuIcons = true;
        UpdateDependencies = true;
    }

    public void Initialize()
    {
        //no initialization needed
    }

    private static void CreateIcons(string folder)
    {
        IEnumerable<InstallerDataAttribute> installables =
            typeof(InstallWindowViewModel).Assembly.GetTypes()
            .Select(x => x.GetCustomAttribute<InstallerDataAttribute>())
            .Where(x => x != null)!;

        foreach (var installable in installables)
        {
            new ShortcutBuilder()
                .WithTargetPath(SelfInterop.CurentProgramPath)
                .WithArguments(installable.Arguments)
                .WithIcon(Path.Combine(AppContext.BaseDirectory, "branding.dll"), installable.IconIndex)
                .Build(Path.Combine(folder, Path.ChangeExtension(installable.Name, ".lnk")));
        }
    }

    [RelayCommand]
    private void Install()
    {
        if (CreateStartMenuIcons)
        {
            var startMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            var mediaFolder = Path.Combine(startMenu, "Media");
            if (!Directory.Exists(mediaFolder))
                Directory.CreateDirectory(mediaFolder);
            CreateIcons(mediaFolder);
        }
        
        if (CreateDesktopIcons)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            CreateIcons(desktop);
        }

        if (AddToPath)
        {
            SystemRegistry.AddFolderToPath(AppContext.BaseDirectory, Logger);
        }

        if (UpdateDependencies)
        {
            _uiFunctions.BringConsoleWindowToFront();
            SelfInterop.RunMediaCommand("update", "all");
        }

        _uiFunctions.InfoMessage("Installation completed.", "Media Installer");
        _uiFunctions.Exit(0);
    }

    [RelayCommand]
    private void Cancel()
    {
        _uiFunctions.Exit(0);
    }
}
