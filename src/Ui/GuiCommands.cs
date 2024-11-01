// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

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
                CommandLine = "imgview {folder}",
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
            yield return new GuiCommand
            {
                ButtonText = "Update ffmpeg",
                Description = "Update the ffmpeg binaries",
                CommandLine = "update ffmpeg",
                Editors = Array.Empty<GuiCommandPart>()
            };
            yield return new GuiCommand
            {
                ButtonText = "Update mpv",
                Description = "Update mpv player",
                CommandLine = "update mpv",
                Editors = Array.Empty<GuiCommandPart>()
            };
            yield return new GuiCommand
            {
                ButtonText = "Update yt-dlp",
                Description = "Update yt-dlp downloader",
                CommandLine = "update yt-dlp",
                Editors = Array.Empty<GuiCommandPart>()
            };
            yield return new GuiCommand
            {
                ButtonText = "Update all",
                Description = "Update all compnents (media+ffmpeg+yt-dlp+mpv)",
                CommandLine = "update all",
                Editors = Array.Empty<GuiCommandPart>()
            };
        }
    }
}
