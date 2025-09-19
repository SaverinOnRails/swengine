using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using HtmlAgilityPack;

using swengine.desktop.Models;

namespace swengine.desktop.Scrapers;

/*I'm not supposed to create a new instance of HttpClient each time. Will Fix this*/
public static class MotionBgsScraper
{
    private static readonly string MotionBgsBase = "https://www.motionbgs.com";
    public static async Task<List<WallpaperResponse>> LatestAsync(int Page)
    {
        string url = $"{MotionBgsBase}/hx2/latest/{Page}/";
        var http = Singleton.Client;
        var request = await http.GetAsync(url);
        if (request.IsSuccessStatusCode)
        {
            List<WallpaperResponse> wallpaper_response = new();
            var string_response = await request.Content.ReadAsStringAsync();
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(string_response);
            var a_links = htmlDoc.DocumentNode.SelectNodes("//a");
            foreach (var aLink in a_links)
            {
                string img_src = MotionBgsBase + aLink.SelectSingleNode(".//img").GetAttributeValue("src", "");
                string title = aLink.SelectSingleNode(".//span[@class='ttl']").InnerHtml;
                string src = MotionBgsBase + aLink.GetAttributeValue("href", "");
                wallpaper_response.Add(new()
                {
                    Title = title,
                    Src = src,
                    Thumbnail = img_src
                });
            }
            return wallpaper_response;
        }
        return default;
    }

    public static async Task<Wallpaper> InfoAsync(string Query, string Title)
    {
        var http = Singleton.Client;
        var request = await http.GetAsync(Query);
        if (request.IsSuccessStatusCode)
        {
            string response = await request.Content.ReadAsStringAsync();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            string source_tag = MotionBgsBase + htmlDoc.DocumentNode.SelectSingleNode("//source[@type='video/mp4']")
                .GetAttributeValue("src", null);
            string text_xs =
                htmlDoc.DocumentNode.SelectSingleNode("//div[@class='text-xs']").InnerHtml.Split(" ")[0];
            string download = MotionBgsBase + htmlDoc.DocumentNode.SelectSingleNode("//div[@class='download']")
                .SelectSingleNode(".//a").GetAttributeValue("href", null);
            return
                new Wallpaper()
                {
                    Title = Title,
                    Resolution = text_xs,
                    Preview = source_tag,
                    WallpaperType = WallpaperType.Live,
                    SourceFile = download
                };
        }

        return default;
    }

    public static async Task<List<WallpaperResponse>> SearchAsync(string Query, int Page)
    {

        string url = $"{MotionBgsBase}/search?q={Query}&page={Page}";
        Debug.WriteLine(url);
        List<WallpaperResponse> result = new();
        var http = Singleton.Client;
        var request = await http.GetAsync(url);
        if (request.IsSuccessStatusCode)
        {
            var response = await request.Content.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            var alinks = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='tmb']").SelectNodes(".//a");
            foreach (var alink in alinks)
            {
                string img_src = MotionBgsBase + alink.SelectSingleNode(".//img").GetAttributeValue("src", null);
                string title = alink.GetAttributeValue("title", null);
                string src = MotionBgsBase + alink.GetAttributeValue("href", null);
                result.Add(new()
                {
                    Title = title,
                    Thumbnail = img_src,
                    Src = src,
                });
            };

            return result;
        }

        return default;
    }
}