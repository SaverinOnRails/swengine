using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace swengine.desktop.Helpers;

public static class DownloadHelper
{
    /*
    *DownloadAsync will return the location of the downloaded file if successful, or null if unsuccesful
    **/
    public static async Task<string> DownloadAsync(string Link, string Title, bool NeedsReferrer = false, string Referer = null)
    {
        string HOME = Environment.GetEnvironmentVariable("HOME");
        try
        {
            //check if file is local, if so copy it to required directory and return new path
            if (File.Exists(Link))
            {
                return CopyLocalFile(Link);
            }

            var client = Singleton.Client;
            using var request = new HttpRequestMessage(HttpMethod.Get, Link);
            Debug.WriteLine(Link);
            request.Headers.Add(
              "User-Agent",
              "Mozilla/5.0 (X11; Linux x86_64; rv:130.0) Gecko/20100101 Firefox/130.0");
            if (NeedsReferrer)
            {
                //if provider requires referer
                request.Headers.Referrer = new Uri(Referer);
            }
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            using var s = await response.Content.ReadAsStreamAsync();
            string extension = ".mp4"; // default extension
            if (response.Content.Headers.ContentDisposition != null)
            {
                var filename = response.Content.Headers.ContentDisposition.FileName?.Trim('\"');
                if (!string.IsNullOrEmpty(filename))
                {
                    // Extract the file extension from the filename
                    extension = Path.GetExtension(filename);
                }

            }
            else
            {
                //try to get extension from url. If both options fail then default to mp4
                string[] possibleExts = new[] { "jpg", "png", "mp4", "gif" };
                string ext = Link.Split(".").Last();
                if (ext != null && possibleExts.Contains(ext))
                {
                    extension = "." + ext;
                }
            }
            //create preconvert directory to store raw MP4s before being passed over to FFMPEG
            Directory.CreateDirectory(HOME + "/Pictures/wallpapers/preconvert");
            using var fs = new FileStream($"{HOME}/Pictures/wallpapers/preconvert/{Title}{extension}", FileMode.Create);

            await s.CopyToAsync(fs);
            //if file does not exist in path then download failed. Return false
            if (!File.Exists($"{HOME}/Pictures/wallpapers/preconvert/{Title}{extension}"))
            {
                return null;
            }
            return $"{HOME}/Pictures/wallpapers/preconvert/{Title}{extension}";
        }
        catch
        {
#if __DEBUG__
            throw;
#endif
            return null;
        }

    }
    private static string CopyLocalFile(string file)
    {
        string HOME = Environment.GetEnvironmentVariable("HOME");
        Directory.CreateDirectory(HOME + "/Pictures/wallpapers/preconvert");

        string filename = Path.GetFileName(file);
        string dest = HOME + "/Pictures/wallpapers/preconvert/" + filename;
        File.Copy(file, dest, true);
        if (!File.Exists(dest))
            return null;
        return dest;
    }

}