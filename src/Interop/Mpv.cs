// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.IO.Pipes;

using Media.Dto;

namespace Media.Interop;

internal sealed class Mpv : IInterop
{
    private Mpv() { }

    private const string MpvBinary = "mpv.exe";

    public static IEnumerable<string> GetSupportedExtensions()
    {
        // Video formats
        yield return ".3gp";
        yield return ".avi";
        yield return ".flv";
        yield return ".mkv";
        yield return ".mov";
        yield return ".mp4";
        yield return ".mpeg";
        yield return ".mpg";
        yield return ".ogv";
        yield return ".ts";
        yield return ".webm";
        yield return ".wmv";
        yield return ".m2ts";
        yield return ".mts";
        yield return ".m4v";
        // Audio formats
        yield return ".aac";
        yield return ".ac3";
        yield return ".aiff";
        yield return ".alac";
        yield return ".ape";
        yield return ".au";
        yield return ".dts";
        yield return ".flac";
        yield return ".m4a";
        yield return ".mp3";
        yield return ".oga";
        yield return ".ogg";
        yield return ".opus";
        yield return ".ra";
        yield return ".tak";
        yield return ".tta";
        yield return ".wav";
        yield return ".wma";
        // Playlist formats
        yield return ".m3u";
        yield return ".m3u8";
        yield return ".pls";
        // Image formats
        yield return ".jpg";
        yield return ".jpeg";
        yield return ".png";
        yield return ".bmp";
        yield return ".gif";
        yield return ".tiff";
        yield return ".webp";
    }

    public static bool TryGetInstalledPath(out string toolPath)
    {
        toolPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MpvBinary);
        return File.Exists(toolPath);
    }

    public static void EnsureIsInstalled()
    {
        if (!TryGetInstalledPath(out _))
        {
            throw new InvalidOperationException("mpv not found.");
        }
    }

    public static int Start(string commandLine)
    {
        if (!TryGetInstalledPath(out string mpvPath))
        {
            throw new InvalidOperationException("mpv not found.");
        }

        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = mpvPath,
                Arguments = commandLine,
                UseShellExecute = false,
            }
        };

        process.Start();
        return process.Id;
    }

    public static async Task<MpvIpcResponse?> SendCommand(string pipeName, string[] payload)
    {
        var commandObject = new
        {
            command = payload
        };

        string json = JsonSerializer.Serialize(commandObject);

        using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
        {
            using (var writer = new StreamWriter(client))
            {
                await writer.WriteLineAsync(json);
            }
            using (var reader = new StreamReader(client, Encoding.UTF8))
            {
                string? response = await reader.ReadLineAsync();
                if (response != null)
                {
                    return JsonSerializer.Deserialize<MpvIpcResponse>(response);
                }
            }
        }
        return null;
    }
}
