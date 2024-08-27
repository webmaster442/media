// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Net.Mime;

using EmbedIO;

using Media.Embedded;
using Media.Infrastructure;

namespace Media.Interop;

internal sealed class MpvWebControllerApp : IDisposable
{
    private readonly WebApp _app;
    private readonly int _processId;
    private readonly string _pipeName;

    public MpvWebControllerApp(int processId, string pipeName)
    {
        _processId = processId;
        _pipeName = pipeName;
        _app = new WebApp(12345);
        _app.AddEmbeddedFile("/", EmbeddedResources.MpvController, MediaTypeNames.Text.Html);
        _app.AddEmbeddedFile("/index.html", EmbeddedResources.MpvController, MediaTypeNames.Text.Html);
        _app.AddEmbeddedFile("/mpv-logo-128.png", EmbeddedResources.MpvLogo, MediaTypeNames.Image.Png);
        _app.AddEmbeddedFile("/style.css", EmbeddedResources.StyleCss, MediaTypeNames.Text.Css);

        _app.AddGetRoute("/action/windowed", (context) => PerformCommand(context, MpvIpcCommandFactory.Fullscreen(false)));
        _app.AddGetRoute("/action/fullscreen", (context) => PerformCommand(context, MpvIpcCommandFactory.Fullscreen(true)));
        _app.AddGetRoute("/action/play", (context) => PerformCommand(context, MpvIpcCommandFactory.Play()));
        _app.AddGetRoute("/action/pause", (context) => PerformCommand(context, MpvIpcCommandFactory.Pause()));
        _app.AddGetRoute("/action/seek-plus", (context) => PerformCommand(context, MpvIpcCommandFactory.SeekRelative(-15)));
        _app.AddGetRoute("/action/seek-minus", (context) => PerformCommand(context, MpvIpcCommandFactory.SeekRelative(+15)));

        _app.AddGetRoute("/action/playlist-previous", (context) => PerformCommand(context, MpvIpcCommandFactory.PlayListPrevious()));
        _app.AddGetRoute("/action/playlist-next", (context) => PerformCommand(context, MpvIpcCommandFactory.PlayListNext()));

        _app.AddGetRoute("/action/volume-plus", (context) => PerformCommand(context, MpvIpcCommandFactory.VolumeRelative(-10)));
        _app.AddGetRoute("/action/volume-minus", (context) => PerformCommand(context, MpvIpcCommandFactory.VolumeRelative(+10)));
        _app.AddGetRoute("/action/volume-mute", (context) => PerformCommand(context, MpvIpcCommandFactory.Mute(true)));
        _app.AddGetRoute("/action/volume-unmute", (context) => PerformCommand(context, MpvIpcCommandFactory.Mute(false)));
        _app.AddGetRoute("/action/subtitle", (context) => PerformCommand(context, MpvIpcCommandFactory.CycleSubtitle()));
        _app.AddGetRoute("/action/audio", (context) => PerformCommand(context, MpvIpcCommandFactory.CycleAudio()));

    }

    private async Task PerformCommand(IHttpContext context, string[] payload)
    {
        if (!IsRunning())
        {
            context.Response.StatusCode = 500;
            await context.SendDataAsync("Mpv is not running");
            return;
        }
        Dto.MpvIpcResponse? result = await Mpv.SendCommand(_pipeName, payload);
        if (result?.IsSuccess == true)
        {
            context.Response.StatusCode = 200;
            await context.SendDataAsync("Ok");
        }
        else
        {
            context.Response.StatusCode = 500;
            await context.SendDataAsync(result?.Error ?? "error");
        }

    }

    private bool IsRunning()
    {
        Process? process = Process.GetProcesses().FirstOrDefault(p => p.Id == _processId);
        if (process == null) return false;
        return !process.HasExited;
    }

    public async Task RunAsync(CancellationToken token)
    {
        var adresses = string.Join("\r\n", _app.GetListenUrls());
        Terminal.InfoText("Server listening on adresses: ");
        Terminal.InfoText(adresses);
        await _app.RunAsync(token);
    }

    public void Dispose()
    {
        _app.Dispose();
    }
}
