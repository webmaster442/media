// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using Media.Interop;

namespace Media.Ui.Converters;

internal sealed class FileTypeToColorConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FileRecognizer.FileType fileType)
        {
            return fileType switch
            {
                FileRecognizer.FileType.Image => Brushes.LightGreen,
                FileRecognizer.FileType.Video => Brushes.LightBlue,
                FileRecognizer.FileType.Playlist => Brushes.LightYellow,
                FileRecognizer.FileType.Audio => Brushes.LightCoral,
                _ => Brushes.White,
            };
        }
        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
