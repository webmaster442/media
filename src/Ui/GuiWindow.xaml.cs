using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Media.Ui;
/// <summary>
/// Interaction logic for GuiWindow.xaml
/// </summary>
public partial class GuiWindow : Window
{
    public GuiWindow()
    {
        InitializeComponent();
    }

    private void SetCorrectTabFromTag(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item
            && item.Tag is string str
            && int.TryParse(str, out int index))
        {
            Dispatcher.Invoke(() => Tabs.SelectedIndex = index, DispatcherPriority.Input);
        }
    }
}
