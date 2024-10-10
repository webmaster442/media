// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Net.Http;

using Media.Dto.Github;

namespace Media.Infrastructure;

internal sealed class GithubClient : IDisposable
{
    private readonly JsonSerializerOptions _options;
    private readonly HttpClient _client;

    public GithubClient()
    {
        _options = new JsonSerializerOptions
        {
            Converters =
            {
                new Json.DateOnlyConverter(),
                new Json.IsoDateTimeOffsetConverter(),
                new Json.StateConverter(),
                new Json.TimeOnlyConverter()
            }
        };
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", " Mozilla/5.0 (platform; rv:geckoversion) Gecko/geckotrail appname/appversion Mozilla/5.0 (platform; rv:geckoversion) Gecko/geckotrail Firefox/firefoxversion appname/appversion ");
    }

    public async Task<Release[]> GetReleases(string owner, string repo)
    {
        var url = $"https://api.github.com/repos/{owner}/{repo}/releases";

        using var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var deserialized = JsonSerializer.Deserialize<Release[]>(json, _options);

        return deserialized ?? throw new InvalidOperationException("Data deserialize failed");
    }

    public async Task<string> DownloadAsset(ReleaseAsset asset, Action<long, long> progeress)
    {
        using var response = await _client.GetAsync(asset.BrowserDownloadUrl);
        response.EnsureSuccessStatusCode();

        long length = response.Content.Headers.ContentLength ??= 0;
        long position = 0;

        var path = Path.GetTempFileName();

        using Stream sourceStream = await response.Content.ReadAsStreamAsync();
        using FileStream targetSteam = File.Create(path);

        byte[] buffer = new byte[16 * 1024];
        int read = 0;
        do
        {
            read = await sourceStream.ReadAsync(buffer, 0, buffer.Length);
            await targetSteam.WriteAsync(buffer, 0, read);
            position += read;
            progeress.Invoke(position, length);
        }
        while (read > 0);

        return path;
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
