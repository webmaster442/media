using System.Windows;

namespace Media.Interfaces;

internal interface IWindowManipulator
{
    Size GetWindowSize(Size xamlDefinedWindowSize);
    Point GetWindowStartupLocation(Size workArea, Size windowSize);
}
