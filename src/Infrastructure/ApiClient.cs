// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Net.Http;
using System.Security;

namespace Media.Infrastructure;

internal abstract class ApiClient : IDisposable
{
    protected readonly HttpClient _client;

    public ApiClient()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", " Mozilla/5.0 (platform; rv:geckoversion) Gecko/geckotrail appname/appversion Mozilla/5.0 (platform; rv:geckoversion) Gecko/geckotrail Firefox/firefoxversion appname/appversion ");
    }

    protected virtual void Dispose(bool disposing)
    {
        _client.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public static class ApiUrls
    {
        public const string GithubApi = "https://api.github.com";
        public const string RadioBrowserApi = "http://nl1.api.radio-browser.info/json";
    }
}
