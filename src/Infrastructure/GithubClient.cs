using System.Text.Json;

using FFCmd.Dto.Github;

namespace FFCmd.Infrastructure;

internal class GithubClient
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
        var url = $"https://api.github.com/repos/{owner}/{repo}/releases";
        using var client = new HttpClient();

        client.DefaultRequestHeaders.Add("User-Agent", " Mozilla/5.0 (platform; rv:geckoversion) Gecko/geckotrail appname/appversion Mozilla/5.0 (platform; rv:geckoversion) Gecko/geckotrail Firefox/firefoxversion appname/appversion ");

        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var deserialized = JsonSerializer.Deserialize<Release[]>(json, _options);

        return deserialized ?? throw new InvalidOperationException("Data deserialize failed");
    }
}
