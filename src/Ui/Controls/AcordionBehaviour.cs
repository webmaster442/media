// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace Media.Ui.Controls;

internal sealed class AcordionBehaviour
{
    public static List<Expander> _expanders = new();

    public static readonly DependencyProperty IsAcordionGroupProperty =
        DependencyProperty.RegisterAttached("IsAcordionGroup", typeof(bool), typeof(Expander), new PropertyMetadata(false, OnChange));

    public static bool GetIsAcordionGroup(DependencyObject obj)
        => (bool)obj.GetValue(IsAcordionGroupProperty);

    public static void SetIsAcordionGroup(DependencyObject obj, bool value)
        => obj.SetValue(IsAcordionGroupProperty, value);

    private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Expander expander)
            return;

        if (e.NewValue is false)
        {
            expander.Expanded -= OnExpanderExpand;
            _expanders.Remove(expander);
        }
        else
        {
            expander.Expanded += OnExpanderExpand;
            _expanders.Add(expander);
        }
    }

    private static void OnExpanderExpand(object sender, RoutedEventArgs e)
    {
        if (sender is not Expander expander)
            return;
        foreach (var item in _expanders)
        {
            if (item != expander)
                item.IsExpanded = false;
        }
    }
}
