using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;
using swengine.desktop.Models;

namespace swengine.desktop.Scrapers;
public static class WallHavenScraper
{
    private static readonly string WallHavenBase = "https://www.wallhaven.cc";

    public static async Task<List<WallpaperResponse>> LatestOrSearchAsync(int page = 1, string Function = "latest", string Query = "")
    {
        string url;
        if (Function == "latest")
        {
            url = $"{WallHavenBase}/latest?page={page}";
        }
        else if(Function == "random"){
            url = $"{WallHavenBase}/random?page={page}";
        }
        else
        {
            url = $"{WallHavenBase}/search?q={Query}&categories=110&purity=100&sorting=date_added&order=desc&ai_art_filter=1&page={page}";
        }
        List<WallpaperResponse> responses = new();
        var http = Singleton.Client;
        using var request = await http.GetAsync(url);
        if (request.IsSuccessStatusCode)
        {
            var response = await request.Content.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            var figures = htmlDoc.DocumentNode.SelectNodes("//figure[contains(@class,'thumb')]");
            foreach (var figure in figures)
            {
                string src = figure.SelectSingleNode(".//a").GetAttributeValue("href", null);
                string title = "Wallhaven Wallpaper"; //this isnt easy to get
                string img_src = figure.SelectSingleNode(".//img").GetAttributeValue("data-src", null);
                responses.Add(new()
                {
                    Title = title,
                    Src = src,
                    Thumbnail = img_src
                });
            }

            return responses;
        }
        return default;
    }

    public static async Task<Wallpaper> InfoAsync(string Query)
    {
        var http = Singleton.Client;
        using var request = await http.GetAsync(Query);
        if (request.IsSuccessStatusCode)
        {
            var response = await request.Content.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            string source = htmlDoc.DocumentNode.SelectSingleNode("//img[@id='wallpaper']").GetAttributeValue("src", null);
            string Title = htmlDoc.DocumentNode.SelectSingleNode("//title").InnerHtml;

            return new Wallpaper()
            {
                Title = Title,
                Resolution = null,
                Preview = source,
                SourceFile = source,
                WallpaperType = WallpaperType.Static
            };
        }
        return default;
    }
}