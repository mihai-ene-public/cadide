using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace IDE.Core.Presentation.AutoUpdates;

public class GithubReleasesManager : IReleasesManager
{
    private readonly HttpClient _client;

    private readonly string _owner;
    private readonly string _repo;

    public GithubReleasesManager()
    {
        //these could be in settings
        _owner = "mihai-ene-public";
        _repo = "cadide";

        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("Accept", "*/*");
        _client.DefaultRequestHeaders.Add("User-Agent", "xnocad");
        _client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
    }

    public async Task<ReleaseInfo> GetLatestRelease(int majorVersion, bool includePrerelease)
    {
        var url = $"https://api.github.com/repos/{_owner}/{_repo}/releases";
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Add("Accept", "*/*");
        var releases = await _client.GetFromJsonAsync<IList<GithubRelease>>(url);

        var latestRelease = releases.FirstOrDefault(r => r.TagName.StartsWith($"v{majorVersion}")
                                                      && (!r.IsPrerelease || includePrerelease)
                                                        );

        if (latestRelease != null)
        {
            var releaseInfo = new ReleaseInfo
            {
                Name = latestRelease.Name,
                Version = latestRelease.TagName,
                Description = latestRelease.Description,
            };

            if (latestRelease.Assets?.Count > 0)
            {
                releaseInfo.AssetFileName = latestRelease.Assets[0].Name;
                releaseInfo.ReleaseAssetDownloadUrl = latestRelease.Assets[0].Url;
                releaseInfo.DownloadSize = latestRelease.Assets[0].DownloadSize;
            }

            return releaseInfo;
        }

        return null;
    }
    public async Task<string> DownloadRelease(string downloadUrl, string assetFileName)
    {
        //var url = "https://api.github.com/repos/mihai-ene-public/cadide/releases/assets/79521303";
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Add("Accept", "application/octet-stream");

        var downloadPath = Path.Combine(Path.GetTempPath(), assetFileName);

        await _client.DownloadFile(downloadUrl, downloadPath);

        return downloadPath;
    }

    class GithubRelease
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("body")]
        public string Description { get; set; }

        [JsonPropertyName("published_at")]
        public DateTime PublishedDate { get; set; }

        [JsonPropertyName("prerelease")]
        public bool IsPrerelease { get; set; }

        [JsonPropertyName("assets")]
        public IList<GithubReleaseAsset> Assets { get; set; }

    }

    class GithubReleaseAsset
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("size")]
        public int DownloadSize { get; set; }

    }
}