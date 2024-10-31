using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using Media.Dto.Internals;

namespace Media.Ui.Controls;

internal sealed class GuiParameterRenderer : Control
{
    private const string DirectorySelectTag = "directory";
    private const string FileSelectTag = "file";

    public GuiCommandPart[]? CommandParts
    {
        get { return (GuiCommandPart[])GetValue(CommandPartsProperty); }
        set { SetValue(CommandPartsProperty, value); }
    }

    public static readonly DependencyProperty CommandPartsProperty =
        DependencyProperty.Register("CommandParts", typeof(GuiCommandPart[]), typeof(GuiParameterRenderer), new PropertyMetadata(null, PartsChanged));

    public ObservableDictionary<string, string> EditorPartValues
    {
        get { return (ObservableDictionary<string, string>)GetValue(EditorPartValuesProperty); }
        set { SetValue(EditorPartValuesProperty, value); }
    }

    public static readonly DependencyProperty EditorPartValuesProperty =
        DependencyProperty.Register("EditorPartValues", typeof(ObservableDictionary<string, string>), typeof(GuiParameterRenderer), new PropertyMetadata(new ObservableDictionary<string, string>()));

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
            var text = new TextBlock
            {
                Text = $"{part.Name} ({part.Description})"
            };
            stackPanel.Children.Add(text);
            stackPanel.Children.Add(control);
        }
    }

    private void UnregisterEvents(UIElementCollection children)
    {
        foreach (var child in children)
        {
            if (child is TextBox textBox)
            {
                textBox.LostFocus -= OnTextboxUpdate;
            }
            else if (child is Button button)
            {
                button.Click -= OnButtonClick;
            }
        }
    }

    private Control CreateEditor(GuiCommandPartEditor editor, string name)
    {
        switch (editor)
        {
            case GuiCommandPartEditor.Directory:
                var btn = new Button();
                btn.Name = name;
                btn.Tag = DirectorySelectTag;
                btn.Content = "Select Directory";
                btn.Click += OnButtonClick;
                return btn;
            case GuiCommandPartEditor.File:
                var fileBtn = new Button();
                fileBtn.Name = name;
                fileBtn.Tag = FileSelectTag;
                fileBtn.Content = "Select File";
                fileBtn.Click += OnButtonClick;
                return fileBtn;
            case GuiCommandPartEditor.Text:
                var textBox = new TextBox();
                textBox.Name = name;
                textBox.Tag = name;
                textBox.LostFocus += OnTextboxUpdate;
                return textBox;
            default:
                throw new UnreachableException($"Editor {editor} is not supported");
        }
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (DirectorySelectTag.Equals(button.Tag))
        {
            var directorySelector = new Microsoft.Win32.OpenFolderDialog();
            if (directorySelector.ShowDialog() == true)
            {
                EditorPartValues[button.Name] = directorySelector.FolderName;
                button.Content = $"{Path.GetFileName(directorySelector.FolderName)} ...";
                button.ToolTip = directorySelector.FolderName;
            }
        }
        else if (FileSelectTag.Equals(button.Tag))
        {
            var fileSelector = new Microsoft.Win32.OpenFileDialog();
            if (fileSelector.ShowDialog() == true)
            {
                EditorPartValues[button.Name] = fileSelector.FileName;
                button.Content = $"{Path.GetFileName(fileSelector.FileName)} ...";
                button.ToolTip = fileSelector.FileName;
            }
        }
    }

    private void OnTextboxUpdate(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        EditorPartValues[textBox.Name] = textBox.Text;
    }
}
