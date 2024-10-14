using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Media.Ui.Converters;

internal sealed class TextToColorConverter : MarkupExtension, IValueConverter
{
    private static readonly Color[] Palette = new Color[]
    {
        Color.FromRgb(244, 67, 54),
        Color.FromRgb(233, 30, 99),
        Color.FromRgb(156, 39, 176),
        Color.FromRgb(103, 58, 183),
        Color.FromRgb(63, 81, 181),
        Color.FromRgb(33, 150, 243),
        Color.FromRgb(3, 169, 244),
        Color.FromRgb(0, 188, 212),
        Color.FromRgb(0, 150, 136),
        Color.FromRgb(76, 175, 80),
        Color.FromRgb(139, 195, 74),
        Color.FromRgb(205, 220, 57),
        Color.FromRgb(255, 235, 59),
        Color.FromRgb(255, 193, 7),
        Color.FromRgb(255, 152, 0),
        Color.FromRgb(255, 87, 34),
        Color.FromRgb(121, 85, 72),
        Color.FromRgb(158, 158, 158),
        Color.FromRgb(96, 125, 139),
    };

    private static uint CalculateHash(string str)
    {
        uint hash = 2166136261;
        foreach (char c in str)
        {
            hash ^= c;
            hash *= 16777619;
        }
        return hash;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            uint hash = CalculateHash(str);
            int index = (int)(hash % Palette.Length);
            return Palette[index];
        }
        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}
