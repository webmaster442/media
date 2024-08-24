// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Media.Infrastructure;

internal sealed class WebApp
{
    private readonly WebApplication _app;
    private readonly int _port;

    public WebApp(int port)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Error);
        builder.Logging.AddConsole();
        builder.WebHost.ConfigureKestrel((context, serverOptions) => serverOptions.ListenAnyIP(port));
        _app = builder.Build();
        _port = port;
    }

    public IEnumerable<string> GetListenUrls()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                yield return $"http://{ip}:{_port}";
            }
        }
    }

    public void AddEmbeddedFile(string requestPath,
                                string embeddedName,
                                string mimeType)
    {
        if (string.IsNullOrEmpty(requestPath))
        {
            throw new ArgumentException($"'{nameof(requestPath)}' cannot be null or empty.", nameof(requestPath));
        }

        if (string.IsNullOrEmpty(embeddedName))
        {
            throw new ArgumentException($"'{nameof(embeddedName)}' cannot be null or empty.", nameof(embeddedName));
        }

        if (string.IsNullOrEmpty(mimeType))
        {
            throw new ArgumentException($"'{nameof(mimeType)}' cannot be null or empty.", nameof(mimeType));
        }

        _app.MapGet(requestPath, async (context) =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var data = Embedded.EmbeddedResources.GetFile(embeddedName);
            if (data == null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("File not found.");
                return;
            }
            context.Response.ContentType = "mimeType";
            context.Response.ContentLength = data.Length;
            await data.CopyToAsync(context.Response.Body);
        });
    }

    public void AddGetRoute([StringSyntax("Route")] string endpoint, RequestDelegate handler)
        => _app.MapGet(endpoint, handler);

    public void AddPostRoute([StringSyntax("Route")] string endpoint, RequestDelegate handler)
        => _app.MapPost(endpoint, handler);

    public async Task RunAsync(CancellationToken token)
        => await _app.RunAsync(token);
}
