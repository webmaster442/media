using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

using Media.Dto;
using Media.Infrastructure;
using Media.Interop;

using NMaier.SimpleDlna.Server.Utilities;

namespace Media.Ui;
/// <summary>
/// Interaction logic for DropConvert.xaml
/// </summary>
public partial class DropConvertWindow : Window
{
    private Preset[] _presets;
    private string _selectedPath;
    private readonly FFMpeg _fFMpeg;

    public DropConvertWindow()
    {
        InitializeComponent();
        _presets = Array.Empty<Preset>();
        _fFMpeg = new FFMpeg(new ConfigAccessor());
        _selectedPath = Environment.CurrentDirectory;
        TbCurrentDir.Text = Path.GetFileName(_selectedPath);
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _presets = await Presets.LoadPresetArray();
        var cvs = new CollectionViewSource();
        cvs.Source = _presets;
        cvs.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
        CbPresetSelector.ItemsSource = cvs.View;
    }

    private void BtnBrowse_Click(object sender, RoutedEventArgs e)
    {
        var selector = new Microsoft.Win32.OpenFolderDialog();
        selector.DefaultDirectory = _selectedPath;
        if (selector.ShowDialog() == true)
        {
            _selectedPath = selector.FolderName;
            TbCurrentDir.Text = Path.GetFileName(_selectedPath);
        }
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effects = DragDropEffects.Copy;
        else
            e.Effects = DragDropEffects.None;

        e.Handled = true;
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effects = DragDropEffects.Copy;
        else
            e.Effects = DragDropEffects.None;

        e.Handled = true;
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (CbPresetSelector.SelectedIndex < 0)
        {
            MessageBox.Show("Please select a preset first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
        {
            var scriptFile = Path.Combine(_selectedPath, Path.ChangeExtension(Path.GetFileName(_selectedPath), ".cmd"));
            using (var writer = File.CreateText(scriptFile))
            {
                foreach (var file in files)
                {
                    string cmdLine = CreateCommandLine(file);
                    writer.WriteLine(cmdLine);
                }
            }
            var shouldRun = MessageBox.Show("Do you want to run the generated script?", "Run?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (shouldRun == MessageBoxResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{scriptFile}\"",
                });
            }
        }
    }

    private string CreateCommandLine(string file)
    {
        if (file.Contains('%'))
        {
            //cmd.exe fix
            file = file.Replace("%", "%%");
        }

        var preset = _presets[CbPresetSelector.SelectedIndex];
        var outputFile = Path.Combine(_selectedPath, Path.ChangeExtension(Path.GetFileName(file), preset.Extension));
        var cmdLine = preset.CommandLine.Replace(Preset.InputPlaceHolder, file);
        return _fFMpeg.GetCommandText(cmdLine.Replace(Preset.OutputPlaceHolder, outputFile)); ;
    }
}