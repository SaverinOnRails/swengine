using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using HtmlAgilityPack;

using swengine.desktop.Models;

namespace swengine.desktop.Scrapers;
public class WallpapersCraftScraper
{
    private static readonly string WallpapersCraftBase = "https://wallpaperscraft.com";


    public async static Task<List<WallpaperResponse>> LatestOrSearchAsync(int page = 1, string function = "latest", string query = null)
    {

        string url = function == "latest" ? WallpapersCraftBase + $"/all/page{page}" : WallpapersCraftBase + $"/search/?order=&page={page}&query={query}&size=";
        var http = Singleton.Client;
        using var request = await http.GetAsync(url);
        request.EnsureSuccessStatusCode();
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(await request.Content.ReadAsStringAsync());
        var wallpaper_canvas = htmlDoc.DocumentNode.SelectNodes("//li[@class='wallpapers__item']");
        List<WallpaperResponse> responses = new();
        foreach (var canvas in wallpaper_canvas)
        {
            string src = WallpapersCraftBase + canvas.SelectSingleNode(".//a").GetAttributeValue("href", null);
            string title = canvas.SelectNodes(".//span[@class='wallpapers__info']")[1].InnerText;
            string thumbnail = canvas.SelectSingleNode(".//img").GetAttributeValue("src", null);
            responses.Add(new()
            {
                Title = title,
                Src = src,
                Thumbnail = thumbnail
            });
        }
        return responses;
    }
    public async static Task<Wallpaper> InfoAsync(string query)
    {
        var http = Singleton.Client;
        using var request = await http.GetAsync(query);
        request.EnsureSuccessStatusCode();
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(await request.Content.ReadAsStringAsync());
        string title = htmlDoc.DocumentNode.SelectSingleNode("//h1[contains(@class,'gui-heading')]").InnerText;
        string preview = htmlDoc.DocumentNode.SelectSingleNode("//img[@class='wallpaper__image']").GetAttributeValue("src", null);

        string extension = preview.Split(".").Last();
        //guess 4k resolution
        string sourcefile = String.Join("_", preview.Split("_").Take(preview.Split("_").Length - 1)) + "_1920x1080." + extension;
        return new Wallpaper()
        {
            Title = title,
            Resolution = null,
            Preview = preview,
            SourceFile = sourcefile,
            WallpaperType = WallpaperType.Static
        };
    }

}