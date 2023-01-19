using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;

namespace IDE.Core.Presentation.Packaging;

public class GithubPackageRepository : IGithubPackageRepository
{
    private readonly HttpClient _client;

    public GithubPackageRepository()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("Accept", "*/*");
        _client.DefaultRequestHeaders.Add("User-Agent", "xnocad GithubPackageManager");
        _client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
    }
    public async Task<IList<PackageInfo>> GetPackages(string packageSource)
    {
        //packageSource should be the raw index.json url from github
        //e.g: https://github.com/mihai-ene-public/xnocad.packages.index/raw/main/index.json

        IList<PackageInfo> packages = new List<PackageInfo>();

        var indexPackages = await GetIndexPackages(packageSource);

        foreach (var indexPackage in indexPackages)
        {
            var pId = indexPackage.PackageId;
            var pDesc = indexPackage.Description;
            var pIcon = indexPackage.Icon;
            var pProjectUrl = indexPackage.ProjectUrl;
            var pAuthors = indexPackage.Authors;
            var pTags = indexPackage.Tags;
            //var pFolder = indexPackage.PackageFolder;

            if (indexPackage.Versions != null)
            {
                foreach (var version in indexPackage.Versions)
                {
                    packages.Add(new PackageInfo
                    {
                        PackageId = pId,
                        Authors = pAuthors,
                        Description = pDesc,
                        Icon = pIcon,
                        PackageSource = packageSource,
                        ProjectUrl = pProjectUrl,
                        Tags = pTags,
                        Version = version,
                    });
                }
            }
        }

        return packages;
    }

    private async Task<IList<PackageInfoIndex>> GetIndexPackages(string packageSource)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var indexFileContents = await _client.GetStringAsync(packageSource);
        var indexPackages = JsonSerializer.Deserialize<IList<PackageInfoIndex>>(indexFileContents, jsonOptions);
        return indexPackages;
    }

    public async Task RestorePackage(PackageInfoRef packageInfoRef, string destinationFolderPath)
    {
        //ensure created folder
        var packageFolderPath = Path.Combine(destinationFolderPath, packageInfoRef.PackageId, packageInfoRef.PackageVersion);

        var packInfoFile = Path.Combine(packageFolderPath, $"{packageInfoRef.PackageId}.info");
        if (File.Exists(packInfoFile))
        {
            //this package is already restored;
            return;
        }

        Directory.CreateDirectory(packageFolderPath);

        //download package
        var packageFileName = $"{packageInfoRef.PackageId}.{packageInfoRef.PackageVersion}.package";
        var packageSourceFilePath = Path.Combine(packageInfoRef.PackageSource, packageFileName);
        var packageDestinationFilePath = Path.Combine(packageFolderPath, packageFileName);

        var packages = await GetIndexPackages(packageInfoRef.PackageSource);
        var package = packages.FirstOrDefault(p => p.PackageId == packageInfoRef.PackageId);

        if (package == null || !package.Versions.Any(p => p != null && p.Equals(packageInfoRef.PackageVersion, StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exception($"Could not restore package: {packageInfoRef.PackageId}.{packageInfoRef.PackageVersion}");
        }

        var downloadUrl = $"{package.PackageFolder}/{packageFileName}";
        await DownloadFile(downloadUrl, packageDestinationFilePath);

        ZipFile.ExtractToDirectory(packageDestinationFilePath, packageFolderPath, true);
    }

    private async Task<string> DownloadFile(string downloadUrl, string downloadPath)
    {
        using (var downloadStream = await _client.GetStreamAsync(downloadUrl))
        {
            using (var fs = new FileStream(downloadPath, FileMode.Create))
            {
                await downloadStream.CopyToAsync(fs);
            }
        }

        return downloadPath;
    }

    public class PackageInfoIndex
    {
        public string PackageId { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string ProjectUrl { get; set; }
        public string Authors { get; set; }
        public string Tags { get; set; }
        public string PackageFolder { get; set; }

        public IList<string> Versions { get; set; } = new List<string>();

    }
}
