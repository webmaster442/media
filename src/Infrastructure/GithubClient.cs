using System.Text.Json;

namespace FFCmd.Infrastructure;

internal class GithubClient
{
    private readonly JsonSerializerOptions _options;

    //https://api.github.com/repos/BtbN/FFmpeg-Builds/releases
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
}
