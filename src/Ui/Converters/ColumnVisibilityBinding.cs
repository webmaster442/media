using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Media.Ui.Converters;

internal sealed class VisibilityToBooleanConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
            return visibility == Visibility.Visible;

        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool val)
            return val ? Visibility.Visible : Visibility.Collapsed;

        return Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) 
        => this;
}
