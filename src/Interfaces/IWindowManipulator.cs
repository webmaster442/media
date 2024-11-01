// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows;

namespace Media.Interfaces;

internal interface IWindowManipulator
{
    Size GetWindowSize(Size xamlDefinedWindowSize);
    Point GetWindowStartupLocation(Size workArea, Size windowSize);
}
