using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Media.Ui.Converters;

internal sealed class LeftMarginMultiplierConverter : MarkupExtension, IValueConverter
{
    public static int GetDepth(TreeViewItem item)
    {
        TreeViewItem? parent;
        while ((parent = GetParent(item)) != null)
        {
            return GetDepth(parent) + 1;
        }
        return 0;
    }

    private static TreeViewItem? GetParent(TreeViewItem item)
    {
        var parent = VisualTreeHelper.GetParent(item);

        while (!(parent is TreeViewItem || parent is TreeView))
        {
            if (parent == null) return null;
            parent = VisualTreeHelper.GetParent(parent);
        }
        return parent as TreeViewItem;
    }

    public double Length { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TreeViewItem item)
            return new Thickness(0);

        return new Thickness(Length * GetDepth(item), 0, 0, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
