using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using swengine.desktop.Models;
namespace swengine.desktop.Scrapers;
public static class DesktopHutScraper {
    private static readonly string _base = "https://www.desktophut.com/";
    public async static Task<List<WallpaperResponse>> LatestOrSearchAsync(int Page = 1, string Function = "latest", string Query = "") {
        List<WallpaperResponse> responses = new();
        HttpClient client = Singleton.Client;
        string url = Function == "latest" ? $"{_base}?page={Page}" : $"{_base}search/{Query}?page={Page}";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
        request.Headers.Add("accept-language", "en-US,en;q=0.7");
        request.Headers.Add("priority", "u=0, i");
        request.Headers.Add("sec-ch-ua", "\"Brave\";v=\"135\", \"Not-A.Brand\";v=\"8\", \"Chromium\";v=\"135\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Linux\"");
        request.Headers.Add("sec-fetch-dest", "document");
        request.Headers.Add("sec-fetch-mode", "navigate");
        request.Headers.Add("sec-fetch-site", "none");
        request.Headers.Add("sec-fetch-user", "?1");
        request.Headers.Add("sec-gpc", "1");
        request.Headers.Add("upgrade-insecure-requests", "1");
        request.Headers.Add("user-agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
        request.Headers.Add("cookie", "u9nb0n6z9v_history=|https://www.desktophut.com/green-haired-warrior-in-forest-4527%2Chttps://www.desktophut.com/images/uVKZ9BcS2d-Cutie2Rhea1.webp%2CGreen%20Haired%20Warrior%20in%20Forest|https://www.desktophut.com/skeleton-gamer-9307%2Chttps://www.desktophut.com/images/Spw0ABeIDT-Sequenc8484fff2Prob4.webp%2CSkeleton%20Gamer%20LoFi%20Live%20Wallpaper|https://www.desktophut.com/cosmic-energy-ring-3568%2Chttps://www.desktophut.com/images/JUh0YGeRBn-Wallpaper22Prob4.webp%2CCosmic%20Energy%20Ring%20Live%20Wallpaper|https://www.desktophut.com/dark-magic-live-wallpaper%2Chttps://www.desktophut.com/images/1707421750.webp%2CDark%20Magic%20Live%20Wallpaper; XSRF-TOKEN=eyJpdiI6IjRnbXErTEZRTmZOMlp3Zk5jYnAxN0E9PSIsInZhbHVlIjoiN25sNFhlMlJ5QlhLSUF2M0dxNktBRktKd1Y0NzlqeDNjSWxuaTJtT3BwQ1JDMnpueUgwWDhmaTljbEZ6clRpZFRBd1lJNndmNDFVQlltRDcwcHZESHUxYWRsbXhISTMvcGFyMCtCbjNZZmNEam1pMjg3cXN6WG5sdWJUdVczRkwiLCJtYWMiOiI4MTkzMDY5NmYyNTk5NDYzZDZmNDNkZDNiMWZlM2JiNTYzZWM1YzAwY2Y1MjRkMThjNzYzZjY5MmEwN2E3NmYxIiwidGFnIjoiIn0%3D; app_portal_session=eyJpdiI6Ink4azFwSGZMYjJwWTFxaHEzSFpiT0E9PSIsInZhbHVlIjoiUEthdVRwL0MzQm5WQk15QnE5am5KeUVsTGFEWEg3TE9jM0FuUEhxWDV2RkFqQ2MrMm1ud003SHRteGlTMksyL0Npd3QzVk9VRjVRTkM3YUg2R1FkWUtObDlCeHNSTlpEVHpxS2xwWnNjN0FhdmVyS1p1a1N2N2lnYXc3bWtLMmsiLCJtYWMiOiIwMTkwZDkxMTUzMGE3ZTBkY2FmMzk0Mjg4ZjY3NmJmOWJhZDYwMDQ0OWQzZTk2N2VlZjE5OGE1ZmY4YjExNzhkIiwidGFnIjoiIn0%3D");
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        HtmlDocument htmlDoc = new();
        htmlDoc.LoadHtml(responseBody);
        var items = htmlDoc.DocumentNode.SelectNodes(".//div[contains(@class,'masonry-item')]");
        foreach (var item in items) {
            //fucking ad block
            try {
                var title = item.SelectSingleNode(".//a").GetAttributeValue("title", null);
                var src = _base + item.SelectSingleNode(".//a").GetAttributeValue("href", null).TrimStart('/');
                var thumbnail = item.SelectSingleNode(".//img").GetAttributeValue("data-src", "");
                responses.Add(new() {
                    Title = title,
                    Src = src,
                    Thumbnail = thumbnail
                });
            } catch { }
        }
        return responses;
    }

    public static async Task<Wallpaper> InfoAsync(string Query) {
        var http = Singleton.Client;
        var request = await http.GetAsync(Query);
        request.EnsureSuccessStatusCode();
        string response = await request.Content.ReadAsStringAsync();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(response);
        var preview = htmlDoc.DocumentNode.SelectSingleNode("//source").GetAttributeValue("src", "");
        var sourceFile = htmlDoc.DocumentNode.SelectNodes("//a[@id='downloadButton']")[1].GetAttributeValue("href", "");
        return new() {
            Title = Query.Split("/").Last(),
            Preview = preview,
            SourceFile = sourceFile
        };
    }
}
