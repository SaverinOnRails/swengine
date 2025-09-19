using System.Net.Http;
using System;
using AvaloniaMpv;
namespace swengine;
///<summary>
///Provider for HttpClient
///</summary>
public static class Singleton {
    private static readonly HttpClientHandler _handler = new() {
        AutomaticDecompression = System.Net.DecompressionMethods.All
    };
    private static readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() => new HttpClient(_handler));
    public static HttpClient Client => _httpClient.Value;

    public static MpvPlayer Player = new();
}