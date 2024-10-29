using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Media.Dto.Internals;

namespace Media.Ui.Controls;

internal sealed class GuiParameterRenderer : Control
{
    public GuiCommandPart[]? CommandParts
    {
        get { return (GuiCommandPart[])GetValue(CommandPartsProperty); }
        set { SetValue(CommandPartsProperty, value); }
    }

    public static readonly DependencyProperty CommandPartsProperty =
        DependencyProperty.Register("CommandParts", typeof(GuiCommandPart[]), typeof(GuiParameterRenderer), new PropertyMetadata(null, PartsChanged));

    private static void PartsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GuiParameterRenderer renderer)
        {
            renderer.RenderParts();
        }
    }

    private void RenderParts()
    {
        if (GetTemplateChild("PART_PANEL") is not StackPanel stackPanel)
        {
            throw new InvalidOperationException("Template must contain a StackPanel with the name PART_PANEL");
        }

        UnregisterEvents(stackPanel.Children);
        stackPanel.Children.Clear();

        if (CommandParts == null || CommandParts.Length == 0)
        {
            stackPanel.Children.Add(new TextBlock { Text = "No parameters needed for command" });
            return;
        }

        foreach (var part in CommandParts)
        {
            var control = CreateEditor(part.Editor, part.Name);
            var label = new Label { Content = part.Name, ToolTip = part.Description };
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(control);
        }
    }

    private void UnregisterEvents(UIElementCollection children)
    {
        foreach (var child in children)
        {
            if (child is TextBox textBox)
            {
            }
            else if (child is Button button)
            {
            }
        }
    }

    private Control CreateEditor(GuiCommandPartEditor editor, string name)
    {
        switch (editor)
        {
            case GuiCommandPartEditor.Directory:
            case GuiCommandPartEditor.Text:
                var textBox = new TextBox();
                textBox.Name = name;
                textBox.Tag = name;
                return textBox;
            default:
                throw new UnreachableException($"Editor {editor} is not supported");
        }
    }
}
