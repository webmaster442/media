using System.Diagnostics;
using System.Net.Mime;

using Media.Infrastructure;

namespace Media.Interop;

public class MpvWebControllerApp
{
    private readonly WebApp _app;
    private readonly int _processId;
    private readonly string _pipeName;

    public MpvWebControllerApp(int processId, string pipeName)
    {
        _processId = processId;
        _pipeName = pipeName;
        _app = new WebApp(12345);
        _app.AddEmbeddedFile("/", "mpvcontroller.html", MediaTypeNames.Text.Html);
        _app.AddEmbeddedFile("/index.html", "index.html", MediaTypeNames.Text.Html);
        _app.AddEmbeddedFile("/request.js", "request.js", MediaTypeNames.Text.JavaScript);
        _app.AddEmbeddedFile("/mpv-logo-128.png", "mpv-logo-128.png", MediaTypeNames.Image.Png);
        _app.AddEmbeddedFile("/w3.css", "w3.css", MediaTypeNames.Text.Css);
        _app.AddGetRoute("/action/play", (context) => PerformCommand(context, MpvIpcCommandFactory.Play()));
        _app.AddGetRoute("/action/pause", (context) => PerformCommand(context, MpvIpcCommandFactory.Pause()));
    }

    private async Task PerformCommand(HttpContext context, string[] payload)
    {
        if (!IsRunning()) return;
        Dto.MpvIpcResponse? result = await Mpv.SendCommand(_pipeName, payload);
        if (result?.IsSuccess == true)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("Ok");
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
        await _app.RunAsync(token);
    }
}
