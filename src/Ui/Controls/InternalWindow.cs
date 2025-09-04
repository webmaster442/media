// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

using Media.Interfaces;

namespace Media.Ui.Controls;

internal sealed class InternalWindow : Control
{
    private sealed class EmptyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public InternalWindow()
    {
        Visibility = Visibility.Collapsed;
        Grid.SetColumnSpan(this, int.MaxValue);
        Grid.SetRowSpan(this, int.MaxValue);
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
    }

    public override void OnApplyTemplate()
    {
        if (GetTemplateChild("PART_Close") is Button closeButton)
        {
            closeButton.Click += OnClose;
        }
    }

    private void OnClose(object sender, RoutedEventArgs e)
        => Hide();

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(InternalWindow), new PropertyMetadata(string.Empty));

    public INotifyPropertyChanged View
    {
        get { return (INotifyPropertyChanged)GetValue(ViewProperty); }
        set { SetValue(ViewProperty, value); }
    }

    public static readonly DependencyProperty ViewProperty =
        DependencyProperty.Register("View", typeof(INotifyPropertyChanged), typeof(InternalWindow), new PropertyMetadata(new EmptyViewModel()));

    public void Show()
    {
        Visibility = Visibility.Visible;
    }

    public void Hide()
    {
        Visibility = Visibility.Collapsed;
        View = new EmptyViewModel();
    }
}
