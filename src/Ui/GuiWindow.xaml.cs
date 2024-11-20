// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.Messaging;

using Media.Ui.Gui;

namespace Media.Ui;
/// <summary>
/// Interaction logic for GuiWindow.xaml
/// </summary>
public partial class GuiWindow : Window
{
    public GuiWindow()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<MenuView.SetTabIndexMessage>(this, SetTabIndex);
    }

    private void SetTabIndex(object recipient, MenuView.SetTabIndexMessage message) 
        => Dispatcher.Invoke(() => Tabs.SelectedIndex = message.Index, DispatcherPriority.Input);
}
