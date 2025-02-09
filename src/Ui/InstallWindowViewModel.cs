using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
        AddToPath = true;
        UpdateDependencies = true;
    }

    public void Initialize()
    {
        //no initialization needed
    }

    private void CreateIcons(string folder)
    {
        new ShortcutBuilder()
            .WithTargetPath(SelfInterop.CurentProgramPath)
            .WithArguments("")
            .WithIcon(SelfInterop.CurentProgramPath, 1)
            .Build(Path.Combine(folder, "Media Cli.lnk"));

        new ShortcutBuilder()
            .WithTargetPath(SelfInterop.CurentProgramPath)
            .WithArguments("gui")
            .WithIcon(SelfInterop.CurentProgramPath, 0)
            .Build(Path.Combine(folder, "Media Gui.lnk"));

        new ShortcutBuilder()
            .WithTargetPath(SelfInterop.CurentProgramPath)
            .WithArguments("convert drop")
            .WithIcon(SelfInterop.CurentProgramPath, 2)
            .Build(Path.Combine(folder, "Media Drop Convert.lnk"));

        new ShortcutBuilder()
            .WithTargetPath(SelfInterop.CurentProgramPath)
            .WithArguments("imgview")
            .WithIcon(SelfInterop.CurentProgramPath, 3)
            .Build(Path.Combine(folder, "Media Image Viewer.lnk"));
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
    }

    [RelayCommand]
    private void Cancel()
    {
        _uiFunctions.Exit(0);
    }
}
