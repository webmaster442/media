using Media.Dto.Internals;

namespace Media.Ui;

internal static class GuiCommands
{
    public static IEnumerable<GuiCommand> Commands
    {
        get
        {
            yield return new GuiCommand
            { 
                ButtonText = "Open Image Viewer",
                Description = "Open the image viewer window",
                CommandLine= "imgview {folder}",
                Editors = new[]
                {
                    new GuiCommandPart
                    {
                        Name = "folder",
                        Description = "folder with images",
                        Editor = GuiCommandPartEditor.Directory
                    }
                }
            };
        }
    }
}
