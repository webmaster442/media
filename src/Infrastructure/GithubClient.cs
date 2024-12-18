﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Github;

namespace Media.Infrastructure;

internal sealed class GithubClient : ApiClient
{
    private readonly JsonSerializerOptions _options;

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
    }

    public async Task<Release[]> GetReleases(string owner, string repo)
    {
        string url = $"{ApiUrls.GithubApi}/repos/{owner}/{repo}/releases";

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
}
