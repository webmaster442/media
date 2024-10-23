// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Markup;

namespace Media.Ui.Converters;

internal sealed partial class TrimConverter : MarkupExtension, IValueConverter
{
    [GeneratedRegex("( ){2,}")]
    internal static partial Regex DoubleSpaceMatcher();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return DoubleSpaceMatcher().Replace(str, "").Trim();
        }
        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return str.Trim();
        }
        return Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
