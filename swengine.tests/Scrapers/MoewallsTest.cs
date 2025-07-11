using swengine.desktop.Models;
using swengine.desktop.Scrapers;

namespace swengine.tests.Scrapers;
[TestClass]
public class MoewallsTest {
    [TestMethod]
    public async Task LatestTest() {
        for (int i = 0; i <= 10; i++) {
            var latest = await MoewallsScraper.LatestOrSearchAsync(i, "latest");

            Assert.IsNotNull(latest);
            Assert.IsInstanceOfType<List<WallpaperResponse>>(latest);
        }
    }

    [TestMethod]
    public async Task InfoTest() {

        var latest = await MoewallsScraper.LatestOrSearchAsync(1, Function: "latest");
        for (int j = 0; j < latest.Count; j++) {
            var info = MoewallsScraper.InfoAsync(latest[j].Src, latest[j].Title);
            var info_object = await info;
            Assert.IsNotNull(info_object);
            Assert.IsInstanceOfType<Wallpaper>(info_object);
        }
    }

    [TestMethod]
    public async Task SearchTest() {
        string[] search_terms = new[] { "nature", "ocean", "space", "city" };
        foreach (var searchTerm in search_terms) {
            var search = await MoewallsScraper.LatestOrSearchAsync(1, "search", searchTerm);
            Assert.IsNotNull(search);
            Assert.IsInstanceOfType<List<WallpaperResponse>>(search);
        }

    }
}
