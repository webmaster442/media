// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace Media.Ui.Controls;

internal sealed class FontIcon : Control
{
    public FontIcon()
    {
        DefaultStyleKey = typeof(FontIcon);
    }

    public IconGlyph Glyph
    {
        get { return (IconGlyph)GetValue(GlyphProperty); }
        set { SetValue(GlyphProperty, value); }
    }

    public static readonly DependencyProperty GlyphProperty =
        DependencyProperty.Register("Glyph", typeof(IconGlyph), typeof(FontIcon), new PropertyMetadata(IconGlyph.GlobalNavigationButton));
}
