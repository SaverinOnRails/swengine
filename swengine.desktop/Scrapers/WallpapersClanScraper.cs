using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using swengine.desktop.Models;
using HtmlAgilityPack;
using System.Linq;
namespace swengine.desktop.Scrapers;
public static class WallpapersClanScraper
{
    private static readonly string WpcBase = "https://wallpapers-clan.com/desktop-wallpapers/";


    public async static Task<List<WallpaperResponse>> LatestAsync(int Page)
    {
        List<WallpaperResponse> responses = new();

        HttpClient client = Singleton.Client;

        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://wallpapers-clan.com/wp-admin/admin-ajax.php");

        request.Headers.Add("User-Agent", "Mozilla/5.0 (iPad; CPU OS 14_7_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.2 Mobile/15E148 Safari/604.1");
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
        // request.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");
        request.Headers.Add("Origin", "https://wallpapers-clan.com");
        request.Headers.Add("Sec-GPC", "1");
        request.Headers.Add("Connection", "keep-alive");
        request.Headers.Add("Referer", "https://wallpapers-clan.com/desktop-wallpapers/");
        request.Headers.Add("Cookie", "visited=1; __wpdm_client=1f0d75c7a3ad2339294b1ba98d53a3b3; cf_clearance=0Q83s8eueBjfPnXaZ11rKQQBR4okeLRRs1Apb1bj7n4-1731654980-1.2.1.1-Uw2wlU0Z8kZQlIkaqBqwWKRPSMJ8KPkhvIwyAdYD.b8UGrxmU8vFJZBYX88uoWyKr708FXFFehlOXnojMWFkp_g9zc6GoLnpWvzXrXaRfN0FeSOfBlYYdf1d.1V.3uVsC0a1IFJMeWJqsO2dYDfluUhwou0W0nhz_7LHZ0CxTsxYUmaMF5XHjdcmDQxxnXK9PB1VRHUgxAq3.WEyQe9KNsYss1yqzBIn9KyJx4HRIuiUYDzWeSyg.pgl6kktBaPONxL0Y2MI5w3_Ymf5XR5dGZYp5uD.lRImBieFUQBZ2e0Jg5pl5ULtDn.UjF0a.ycKuHjeHsSDEdUzxSjaksSO0Q");
        request.Headers.Add("Sec-Fetch-Dest", "empty");
        request.Headers.Add("Sec-Fetch-Mode", "cors");
        request.Headers.Add("Sec-Fetch-Site", "same-origin");
        request.Headers.Add("TE", "trailers");
        //formdata content, with the requested page
        request.Content = new StringContent($"action=boldlab_get_new_posts&options%5Bplugin%5D=boldlab_core&options%5Bmodule%5D=post-types%2Fdwallpapers%2Fshortcodes&options%5Bshortcode%5D=dwallpapers-list&options%5Bpost_type%5D=dwallpapers&options%5Bnext_page%5D={Page}&options%5Bmax_pages_num%5D=331&options%5Bshow_category%5D=no&options%5Bbehavior%5D=columns&options%5Bimages_proportion%5D=full&options%5Bcolumns%5D=3&options%5Bspace%5D=normal&options%5Bcolumns_responsive%5D=predefined&options%5Bcolumns_1440%5D=3&options%5Bcolumns_1366%5D=3&options%5Bcolumns_1024%5D=3&options%5Bcolumns_768%5D=3&options%5Bcolumns_680%5D=3&options%5Bcolumns_480%5D=3&options%5Bposts_per_page%5D=12&options%5Borderby%5D=date&options%5Border%5D=DESC&options%5Badditional_params%5D=tax&options%5Blayout%5D=info-below&options%5Bhover_animation_info-below%5D=tilt&options%5Bhover_animation_info-follow%5D=follow&options%5Bhover_animation_info-on-hover%5D=direction-aware&options%5Btitle_tag%5D=h4&options%5Bcustom_padding%5D=no&options%5Benable_filter%5D=yes&options%5Bpagination_type%5D=infinite-scroll&options%5Bloading_animation%5D=no&options%5Bobject_class_name%5D=BoldlabCoredwallpapersListShortcode&options%5Btaxonomy_filter%5D=dwallpapers-category&options%5Bspace_value%5D=15&options%5Bjustified_attr%5D=%7B%22rowHeight%22%3A%22%22%2C%22spaceBetween%22%3A15%7D");
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded; charset=UTF-8");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        
        WallpapersClanJsonResonse jsonResponse = System.Text.Json.JsonSerializer.Deserialize<WallpapersClanJsonResonse>(responseBody, JsonContext.Default.WallpapersClanJsonResonse);
        HtmlDocument htmlDoc = new();
        htmlDoc.LoadHtml(jsonResponse.data);

        var articleNodes = htmlDoc.DocumentNode.SelectNodes("//article");
        foreach (var article in articleNodes)
        {
            var last_a_tag = article.SelectNodes(".//a").Last();
            string title = last_a_tag.InnerText;
            string src = last_a_tag.GetAttributeValue("href", null);
            string thumbnail = article.SelectSingleNode(".//img").GetAttributeValue("src", null);
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
        string title = htmlDoc.DocumentNode.SelectSingleNode("//h1[contains(@class,'entry-title')]").InnerText;
        string SourceFile = htmlDoc.DocumentNode.SelectSingleNode("//a[contains(@class,'wpdm-download-link')]").GetAttributeValue("data-downloadurl", "null-source");
        string preview = htmlDoc.DocumentNode.SelectSingleNode("//img[contains(@class,'size-full') and contains(@class,'attachment-full')]").GetAttributeValue("data-lazy-srcset", null)?.Split(" ")[0];
        if (preview == null)
        {
            preview = htmlDoc.DocumentNode.SelectSingleNode("//img[contains(@class,'size-full') and contains(@class,'attachment-full')]").GetAttributeValue("data-lazy-src", null)?.Split(" ")[0];

        }
        return new()
        {
            Title = title,
            Resolution = null,
            Preview = preview,
            SourceFile = SourceFile,
            WallpaperType = WallpaperType.Static,
        };
    }
    public async static Task<List<WallpaperResponse>> SearchAsync(string query, int page)
    {
        List<WallpaperResponse> responses = new();
        string url = $"https://wallpapers-clan.com/page/{page}/?s={query}";
        HttpClientHandler handler = new HttpClientHandler();
        handler.AutomaticDecompression = DecompressionMethods.All;
        using HttpClient client = new HttpClient(handler);
        using HttpRequestMessage request =
            new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add(
            "User-Agent",
            "Mozilla/5.0 (X11; Linux x86_64; rv:130.0) Gecko/20100101 Firefox/130.0");
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(responseBody);

        var articleNodes = htmlDoc.DocumentNode.SelectNodes("//article");
        foreach (var article in articleNodes)
        {
            string title = article.SelectSingleNode(".//a[@class='qodef-e-title-link']").InnerText;
            //skip if article is not a desktop wallpaper
            if (!title.Contains("Desktop Wallpaper")) continue;

            string preview = article.SelectSingleNode(".//img").GetAttributeValue("src", null);
            string src = article.SelectSingleNode(".//a").GetAttributeValue("href", null);
            responses.Add(new()
            {
                Title = title,
                Src = src,
                Thumbnail = preview,
            });
        }
        return responses;
    }
}

[JsonSerializable(typeof(WallpapersClanJsonResonse))]
public partial class JsonContext : JsonSerializerContext
{

}

public struct WallpapersClanJsonResonse
{
    public string? success { get; set; }
    public string? message { get; set; }
    public string? data { get; set; }
}
