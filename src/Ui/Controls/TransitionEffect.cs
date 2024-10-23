// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Media.Ui.Controls;

internal class TransitionEffect : ShaderEffect
{
    public static readonly DependencyProperty InputProperty 
        = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(TransitionEffect), 0);

    public static readonly DependencyProperty ProgressProperty
        = DependencyProperty.Register("Progress", typeof(double), typeof(TransitionEffect), new UIPropertyMetadata(0D, PixelShaderConstantCallback(0)));
    
    public static readonly DependencyProperty ModeProperty
        = DependencyProperty.Register("Mode", typeof(double), typeof(TransitionEffect), new UIPropertyMetadata(1D, PixelShaderConstantCallback(1)));
    
    public static readonly DependencyProperty SlideVectorProperty
        = DependencyProperty.Register("SlideVector", typeof(Point), typeof(TransitionEffect), new UIPropertyMetadata(new Point(1D, 0D), PixelShaderConstantCallback(2)));
    
    public static readonly DependencyProperty Texture2Property
        = ShaderEffect.RegisterPixelShaderSamplerProperty("Texture2", typeof(TransitionEffect), 1);
    
    public TransitionEffect()
    {
        PixelShader pixelShader = new();
        pixelShader.UriSource = new Uri("/Media;component/Embedded/TransitionEffect.ps", UriKind.Relative);
        PixelShader = pixelShader;

        UpdateShaderValue(InputProperty);
        UpdateShaderValue(ProgressProperty);
        UpdateShaderValue(ModeProperty);
        UpdateShaderValue(SlideVectorProperty);
        UpdateShaderValue(Texture2Property);
    }

    public Brush Input
    {
        get => (Brush)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }
    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public double Mode
    {
        get => (double)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public Point SlideVector
    {
        get => (Point)GetValue(SlideVectorProperty);
        set => SetValue(SlideVectorProperty, value);
    }
    public Brush Texture2
    {
        get => (Brush)GetValue(Texture2Property);
        set => SetValue(Texture2Property, value);
    }
}
