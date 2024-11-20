// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows.Controls;

namespace Media.Ui.Controls;

internal class AsyncBlocker : Control
{
    public AsyncBlocker()
    {
        Visibility = System.Windows.Visibility.Collapsed;
        Panel.SetZIndex(this, 1000);
        Grid.SetColumnSpan(this, int.MaxValue);
        Grid.SetRowSpan(this, int.MaxValue);
    }

    public void Show()
    {
        Visibility = System.Windows.Visibility.Visible;
        if (GetTemplateChild("PART_PROGRESS") is ProgressBar progressBar)
        {
            progressBar.IsIndeterminate = true;
        }
    }

    public void Hide()
    {
        if (GetTemplateChild("PART_PROGRESS") is ProgressBar progressBar)
        {
            progressBar.IsIndeterminate = false;
        }
        Visibility = System.Windows.Visibility.Collapsed;
    }
}
