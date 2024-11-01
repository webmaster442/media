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

    public void Start(MpvCommandBuilder mpvCommand)
        => Start(mpvCommand.Build());

    protected override string? GetExternalPath()
        => _configAccessor.GetExternalMpvPath();
}