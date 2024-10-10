using System.Windows;
using System.Windows.Data;

using Media.Dto;
using Media.Infrastructure;

namespace Media.Ui;
/// <summary>
/// Interaction logic for DropConvert.xaml
/// </summary>
public partial class DropConvertWindow : Window
{
    private Preset[] _presets;
    private string _selectedPath;

    public DropConvertWindow()
    {
        InitializeComponent();
        _presets = Array.Empty<Preset>();
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
}
