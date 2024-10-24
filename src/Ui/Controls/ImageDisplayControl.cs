// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Media.Ui.Controls;

internal sealed class ImageDisplayControl : Control
{
    public string ImagePath
    {
        get { return (string)GetValue(ImagePathProperty); }
        set { SetValue(ImagePathProperty, value); }
    }

    public static readonly DependencyProperty ImagePathProperty =
        DependencyProperty.Register("ImagePath", typeof(string), typeof(ImageDisplayControl), new PropertyMetadata(string.Empty, DoResize));

    public double ContainerWidth
    {
        get { return (double)GetValue(ContainerWidthProperty); }
        set { SetValue(ContainerWidthProperty, value); }
    }

    public static readonly DependencyProperty ContainerWidthProperty =
        DependencyProperty.Register("ContainerWidth", typeof(double), typeof(ImageDisplayControl), new PropertyMetadata(0d, DoResize));

    public double ContainerHeight
    {
        get { return (double)GetValue(ContainerHeightProperty); }
        set { SetValue(ContainerHeightProperty, value); }
    }

    public static readonly DependencyProperty ContainerHeightProperty =
        DependencyProperty.Register("ContainerHeight", typeof(double), typeof(ImageDisplayControl), new PropertyMetadata(0d, DoResize));

    public SizeMode SizeMode
    {
        get { return (SizeMode)GetValue(SizeModeProperty); }
        set { SetValue(SizeModeProperty, value); }
    }

    public static readonly DependencyProperty SizeModeProperty =
        DependencyProperty.Register("SizeMode", typeof(SizeMode), typeof(ImageDisplayControl), new PropertyMetadata(SizeMode.Fit, DoResize));

    private static void DoResize(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageDisplayControl control)
            control.DoSizing();
    }

    private Image ImageControl = null!;

    public override void OnApplyTemplate()
    {
        if (GetTemplateChild("PART_IMAGE") is not Image img)
            throw new InvalidOperationException("Template doesn't contain image");

        ImageControl = img;
    }

    private void DoSizing()
    {
        if (string.IsNullOrEmpty(ImagePath) || !File.Exists(ImagePath))
            return;

        var imgData = new BitmapImage(new Uri(ImagePath));
        ImageControl.Source = imgData;

        int pictureWidth =  imgData.PixelWidth;
        int pictureHeight = imgData.PixelHeight;

        (int resultWidth, int resultHegiht) = ComputeSize(pictureWidth, pictureHeight);

        Width = resultWidth;
        Height = resultHegiht;
        ImageControl.Width = resultWidth;
        ImageControl.Height = resultHegiht;
    }

    private (int resultWidht, int resultHegiht) ComputeSize(int pictureWidth, int pictureHeight)
    {
        double containerAspectRatio = ContainerWidth / ContainerHeight;
        double pictureAspectRatio = (double)pictureWidth / pictureHeight;

        return SizeMode switch
        {
            SizeMode.Fit => ComputeFit(pictureWidth, pictureHeight, containerAspectRatio, pictureAspectRatio),
            SizeMode.FitWidth => ComputeFitWidth(pictureWidth, pictureHeight, pictureAspectRatio),
            SizeMode.FitHeight => ComputeFitHeight(pictureWidth, pictureHeight, pictureAspectRatio),
            SizeMode.Original => (pictureWidth, pictureHeight),
            _ => throw new UnreachableException("Unknown SizeMode"),
        };
    }

    private (int resultWidht, int resultHegiht) ComputeFitHeight(int pictureWidth,
                                                                 int pictureHeight,
                                                                 double pictureAspectRatio)
    {
        int height =  pictureHeight > ContainerHeight ? (int)ContainerHeight : pictureHeight;
        int width = (int)(height * pictureAspectRatio);
        return (width, height);
    }

    private (int resultWidht, int resultHegiht) ComputeFitWidth(int pictureWidth,
                                                                int pictureHeight,
                                                                double pictureAspectRatio)
    {
        int width = pictureWidth > ContainerWidth ? (int)ContainerWidth : pictureWidth;
        int height = (int)(width / pictureAspectRatio);
        return (width, height);
    }

    private (int resultWidht, int resultHegiht) ComputeFit(int pictureWidth,
                                                           int pictureHeight,
                                                           double containerAspectRatio,
                                                           double pictureAspectRatio)
    {
        int width, height;

        if (containerAspectRatio > pictureAspectRatio)
        {
            height = (int)ContainerHeight;
            width = (int)(height * pictureAspectRatio);
        }
        else
        {
            width = (int)ContainerWidth;
            height = (int)(width / pictureAspectRatio);
        }

        return (width, height);
    }
}
