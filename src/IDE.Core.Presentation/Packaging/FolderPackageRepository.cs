using System.IO;
using System.IO.Compression;
using IDE.Core.Data.Packages;

namespace IDE.Core.Presentation.Packaging;

public class FolderPackageRepository : IFolderPackageRepository
{
    public Task<IList<PackageInfo>> GetPackages(string packageSource)
    {
        IList<PackageInfo> packages = new List<PackageInfo>();
        if (Directory.Exists(packageSource))
        {
            foreach (var packageFile in Directory.EnumerateFiles(packageSource, "*.package"))
            {
                using (var fs = new FileStream(packageFile, FileMode.Open, FileAccess.Read))
                {
                    using (var zip = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        var infoEntry = zip.Entries.FirstOrDefault(e => e.FullName.EndsWith(".info"));
                        if (infoEntry != null)
                        {
                            using (var entryStream = infoEntry.Open())
                            {
                                var packInfo = XmlHelper.GetObjectfromStream<PackageMetadata>(entryStream);

                                if (packInfo != null)
                                {
                                    packages.Add(new PackageInfo
                                    {
                                        PackageId = packInfo.Id,
                                        Authors = packInfo.Authors,
                                        Description = packInfo.Description,
                                        Icon = packInfo.Icon,
                                        PackageSource = packageSource,
                                        ProjectUrl = packInfo.ProjectUrl,
                                        Tags = packInfo.Tags,
                                        Version = packInfo.Version,
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        return Task.FromResult(packages);
    }

    public Task RestorePackage(PackageInfoRef packageInfoRef, string destinationFolderPath)
    {
        //ensure created folder
        var packageFolderPath = Path.Combine(destinationFolderPath, packageInfoRef.PackageId, packageInfoRef.PackageVersion);

        var packInfoFile = Path.Combine(packageFolderPath, $"{packageInfoRef.PackageId}.info");
        if (File.Exists(packInfoFile))
        {
            //this package is already restored;
            return Task.CompletedTask;
        }

        Directory.CreateDirectory(packageFolderPath);

        //download package (just copy)
        var packageFileName = $"{packageInfoRef.PackageId}.{packageInfoRef.PackageVersion}.package";
        var packageSourceFilePath = Path.Combine(packageInfoRef.PackageSource, packageFileName);
        var packageDestinationFilePath = Path.Combine(packageFolderPath, packageFileName);

        File.Copy(packageSourceFilePath, packageDestinationFilePath, true);//it is just easier to overwrite

        ZipFile.ExtractToDirectory(packageDestinationFilePath, packageFolderPath, true);

        return Task.CompletedTask;
    }
}
