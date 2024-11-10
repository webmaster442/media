using System.Windows.Data;
using System.Windows.Markup;

namespace Media.Ui.Converters;

internal sealed class StringFormatConverter : MarkupExtension, IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string format)
        {
            return string.Format(format, values);
        }

        return Binding.DoNothing;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
       => [];

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
