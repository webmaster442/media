using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Media.Interop;

internal static class FileRecognizer
{
    private readonly static HashSet<string> AudioFiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ".aac", ".ac3", ".aiff", ".ape", ".au",
        ".dts", ".flac", ".m4a", ".m4b", ".mp3", 
        ".oga", ".ogg", ".opus", ".ra", ".tak", 
        ".tta", ".wav", ".wma", ".wv", ".weba",
        ".alac", ".amr", ".mp2"
    };

    private readonly static HashSet<string> VideoFiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ".3gp", ".avi", ".flv", ".mkv", ".mov",
        ".mp4", ".mpeg", ".mpg", ".ogv", ".ts",
        ".webm", ".wmv", ".m2ts", ".mts", ".m4v",
        ".vob", ".ts",
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
