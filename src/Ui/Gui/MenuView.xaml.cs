using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.Messaging;

namespace Media.Ui.Gui;
/// <summary>
/// Interaction logic for MenuView.xaml
/// </summary>
public partial class MenuView : UserControl
{
    public MenuView()
    {
        InitializeComponent();
    }

    internal sealed record class SetTabIndexMessage(int Index);

    private void SetCorrectTabFromTag(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item
            && item.Tag is string str
            && int.TryParse(str, out int index))
        {
            WeakReferenceMessenger.Default.Send(new SetTabIndexMessage(index));
        }
    }
}
