// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.IO.Pipes;

using Media.Dto;
using Media.Infrastructure;

namespace Media.Interop;

internal sealed class Mpv : InteropBase
{

    private const string MpvBinary = "mpv.exe";
    private readonly ConfigAccessor _configAccessor;

    public Mpv(ConfigAccessor configAccessor) : base(MpvBinary)
    {
        _configAccessor = configAccessor;
    }

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

    public static async Task<MpvIpcResponse?> SendCommand(string pipeName, string[] payload)
    {
        var commandObject = new
        {
            command = payload
        };

        string json = JsonSerializer.Serialize(commandObject);

        using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
        {
            client.Connect();
            using (var writer = new StreamWriter(client, leaveOpen: true))
            {
                writer.WriteLine(json);
                writer.Flush();
            }
            using (var reader = new StreamReader(client))
            {
                var response = await reader.ReadLineAsync();
                if (response != null)
                {
                    return JsonSerializer.Deserialize<MpvIpcResponse>(response);
                }
            }
        }
        return null;
    }

    protected override string? GetExternalPath()
        => _configAccessor.GetExternalMpvPath();
}