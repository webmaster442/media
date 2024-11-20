// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Markup;

using Media.Interop;
using Media.Ui.Controls;

namespace Media.Ui.Converters;

internal sealed class FileTypeToIconConverter : MarkupExtension, IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2
            && values[0] is bool isFolder
            && values[1] is FileRecognizer.FileType fileType)
        {
            if (isFolder)
                return IconGlyph.FolderOpen;

            return fileType switch
            {
                FileRecognizer.FileType.Image => IconGlyph.Photo2,
                FileRecognizer.FileType.Video => IconGlyph.Video,
                FileRecognizer.FileType.Playlist => IconGlyph.BulletedList,
                FileRecognizer.FileType.Audio => IconGlyph.MusicNote,
                FileRecognizer.FileType.Other => (object)IconGlyph.OpenFile,
                _ => throw new UnreachableException(),
            };
        }

        return Binding.DoNothing;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => Array.Empty<object>();

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
