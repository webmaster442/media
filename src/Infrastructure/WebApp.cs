// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Media.Infrastructure;

internal sealed class WebApp
{
    private readonly WebApplication _app;

    public WebApp(int port)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Error);
        builder.Logging.AddConsole();
        builder.WebHost.ConfigureKestrel((context, serverOptions) => serverOptions.ListenAnyIP(port));
        _app = builder.Build();
        Port = port;
    }

    public ILogger Logger => _app.Logger;

    public int Port { get; }

    public IEnumerable<string> GetListenUrls()
    {
        foreach (var (adress, _) in GetIpAdresses())
        {
            yield return $"http://{adress}:{Port}";
        }
    }

    public IEnumerable<(IPAddress adress, IPAddress mask)> GetIpAdresses()
    {
        var ipAdresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .Where(i => i.AddressFamily == AddressFamily.InterNetwork)
            .ToHashSet();

        var ifaceAddrs = NetworkInterface.GetAllNetworkInterfaces()
            .Where(i => i.OperationalStatus == OperationalStatus.Up)
            .SelectMany(x => x.GetIPProperties().UnicastAddresses)
            .Where(x => ipAdresses.Contains(x.Address));

        foreach (var adress in ifaceAddrs)
        {
            yield return (adress.Address, adress.IPv4Mask);
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

    public async Task RunAsync()
        => await _app.RunAsync();
}