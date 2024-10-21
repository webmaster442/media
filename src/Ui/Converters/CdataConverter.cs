// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Markup;

using Media.Dto;

namespace Media.Ui.Converters;
internal partial class CdataConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CData cData)
        {
            return DoubleOrMoreWhiteSpace()
                .Replace(cData, "")
                .Trim();
        }
        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return new CData(str);
        }
        return Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;

    [GeneratedRegex("( ){2,}")]
    private static partial Regex DoubleOrMoreWhiteSpace();
}
