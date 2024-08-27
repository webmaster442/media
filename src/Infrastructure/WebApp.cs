// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;

using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;

using Media.Dto.Internals;

namespace Media.Infrastructure;

internal sealed class WebApp : IDisposable
{
    private readonly WebServer _server;
    
    public WebApp(int port)
    {
        Port = port;
        _server = new WebServer(port);
    }

    public int Port { get; }

    public IEnumerable<string> GetListenUrls()
    {
        var ipAdresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .Where(i => i.AddressFamily == AddressFamily.InterNetwork)
            .ToHashSet();

        foreach (var adress in ipAdresses)
        {
            yield return $"http://{adress}:{Port}";
        }
    }

    public void AddGetRoute(string routePath, RequestHandlerCallback handler)
        => _server.Modules.Add(routePath, new ActionModule(routePath, HttpVerbs.Get, handler));


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

        _server.Modules.Add("embeddedHandler", new ActionModule(requestPath, HttpVerbs.Get, async context =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var data = Embedded.EmbeddedResources.GetFile(embeddedName);
            if (data == null)
            {
                context.Response.StatusCode = 404;
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                var response = "File not found."u8.ToArray();
                context.Response.ContentLength64 = response.Length;
                await context.Response.OutputStream.WriteAsync(response);
                return;
            }
            context.Response.ContentType = "mimeType";
            context.Response.ContentLength64 = data.Length;
            await data.CopyToAsync(context.Response.OutputStream);
        }));
    }

    public async Task RunAsync(CancellationToken token)
        => await _server.RunAsync(token);

    public void Dispose()
    {
        _server.Dispose();
    }
}
