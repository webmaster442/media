using System.Windows.Data;
using System.Windows.Markup;

using Media.Ui.Controls;

namespace Media.Ui.Converters;

internal class GlyphToTextConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IconGlyph glyph)
        {
            return char.ConvertFromUtf32((int)glyph);
        }
        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
