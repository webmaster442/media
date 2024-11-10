using System.Windows.Data;
using System.Windows.Markup;

namespace Media.Ui.Converters;

internal sealed class JoinConverter : MarkupExtension, IMultiValueConverter, IValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return string.Join("", values);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return $"{parameter}\r\n{value}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => [];

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
