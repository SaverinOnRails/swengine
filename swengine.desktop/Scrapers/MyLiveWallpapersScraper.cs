using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using HtmlAgilityPack;

using swengine.desktop.Models;

namespace swengine.desktop.Scrapers;

public static class MyLiveWallpapersScraper
{
    private static readonly string mlwbase = "https://www.mylivewallpapers.com/";

    public async static Task<List<WallpaperResponse>> LatestOrSearchAsync(
        int page = 1, string function = "latest", string query = "")
    {
        string url = function == "latest" ? mlwbase + $"page/{page}"
                                          : mlwbase + $"page/{page}/?s={query}";
        HttpClient client = Singleton.Client;
        using HttpRequestMessage request =
            new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add(
            "User-Agent",
            "Mozilla/5.0 (X11; Linux x86_64; rv:130.0) Gecko/20100101 Firefox/130.0");
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        List<WallpaperResponse> wallpaper_responses = new();
        string responseBody = await response.Content.ReadAsStringAsync();
        string content = await response.Content.ReadAsStringAsync();
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(content);
        // begin scraping
        var posts =
            htmlDoc.DocumentNode.SelectNodes("//a[contains(@class,'post')]");
        if (posts is null)
            return default;
        foreach (var post in posts)
        {
            string img_src = post.GetAttributeValue("style", null)
                                 .Split("background-image: url( ")[1]
                                 .Split(");")[0];

            string title = post.GetAttributeValue("href", null).Split("/")[^2];

            string src = post.GetAttributeValue("href", null);
            wallpaper_responses.Add(
                new() { Title = title, Src = src, Thumbnail = img_src });
        }

        return wallpaper_responses;
    }

    public static async Task<Wallpaper> InfoAsync(string query)
    {
        HttpClient client = Singleton.Client;
        using HttpRequestMessage request =
            new HttpRequestMessage(HttpMethod.Get, query);
        request.Headers.Add(
            "User-Agent",
            "Mozilla/5.0 (X11; Linux x86_64; rv:130.0) Gecko/20100101 Firefox/130.0");
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(responseBody);
        string title =
            htmlDoc.DocumentNode.SelectSingleNode("//h1[@class='post-title']")
                .InnerText;
        string preview =
            htmlDoc.DocumentNode.SelectSingleNode("//source[@type='video/mp4']")?
                .GetAttributeValue("src", null);
        string sourcefile =
            htmlDoc.DocumentNode
                .SelectSingleNode("//a[contains(@class,'wpdm-download-link')]")
                .GetAttributeValue("data-downloadurl", null);
        return new Wallpaper()
        {
            Title = title,
            Resolution = null,
            WallpaperType = WallpaperType.Live,
            Preview = preview,
            SourceFile = sourcefile,
            NeedsReferrer = false,

        };
    }
}