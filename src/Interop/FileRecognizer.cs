// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interop;

internal static class FileRecognizer
{
    private readonly static HashSet<string> AudioFiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ".aac", ".ac3", ".aiff", ".alac",
        ".amr", ".ape", ".atrac", ".au",
        ".caf", ".dts", ".flac", ".gsm",
        ".m4a", ".m4b", ".mka", ".mlp",
        ".mp2", ".mp3", ".oga", ".opus",
        ".ra", ".raw", ".shn", ".tak",
        ".tta", ".voc", ".wav", ".wma",
        ".wv"
    };

    private readonly static HashSet<string> VideoFiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ".3gp", ".asf", ".avi", ".bfi",
        ".f4v", ".flv", ".gxf", ".m4v",
        ".matroska", ".mkv", ".mov", ".mp4",
        ".mpeg", ".mpg", ".mts", ".mxf",
        ".nut", ".ogg", ".ogv", ".rm",
        ".ts", ".vob", ".webm", ".wm", 
        ".wmv", ".yuv"
    };

    private readonly static HashSet<string> ImageFiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ".bmp", ".gif", ".jpeg", ".jpg", ".png", ".webp", ".tiff"
    };

    private readonly static HashSet<string> PlaylistFiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ".m3u", ".m3u8", ".pls"
    };

    public enum FileType
    {
        Other,
        Video,
        Image,
        Audio,
        Playlist,
    }

    public static FileType GetFileType(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        if (AudioFiles.Contains(extension))
            return FileType.Audio;
        if (VideoFiles.Contains(extension))
            return FileType.Video;
        if (ImageFiles.Contains(extension))
            return FileType.Image;
        if (PlaylistFiles.Contains(extension))
            return FileType.Playlist;
        return FileType.Other;
    }

    public static IEnumerable<string> GetMpvSupportedExtensions()
    {
        return AudioFiles
            .Concat(VideoFiles)
            .Concat(ImageFiles)
            .Concat(PlaylistFiles);
    }

    public static bool IsMpvSupportedType(this FileType fileType)
    {
        return fileType == FileType.Audio
            || fileType == FileType.Video
            || fileType == FileType.Playlist;
    }

    public static bool IsDropConvertSupported(string file)
    {
        var type = GetFileType(file);
        return type == FileType.Audio || type == FileType.Video;
    }
}
