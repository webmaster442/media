using System.Windows;
using System.Windows.Controls;

using Media.Interfaces;

namespace Media.Ui.Controls;

internal sealed class InternalWindow : Control
{
    private sealed class EmptyViewModel : IViewModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Initialize()
        {
        }
    }

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(InternalWindow), new PropertyMetadata(string.Empty));

    public IViewModel View
    {
        get { return (IViewModel)GetValue(ViewProperty); }
        set { SetValue(ViewProperty, value); }
    }

    public static readonly DependencyProperty ViewProperty =
        DependencyProperty.Register("View", typeof(IViewModel), typeof(InternalWindow), new PropertyMetadata(new EmptyViewModel()));

    public void Show()
    {
        View?.Initialize();
        Visibility = Visibility.Visible;
    }

    public void Hide()
    {
        Visibility = Visibility.Collapsed;
        View = new EmptyViewModel();
    }
}
