﻿using System.Windows;


namespace Media.Ui.Controls;

/// <summary>
/// Provides the size of items displayed in an VirtualizingPanel.
/// </summary>
public interface IItemSizeProvider
{
    /// <summary>
    /// Gets the size for the specified item.
    /// </summary>
    Size GetSizeForItem(object item);
}