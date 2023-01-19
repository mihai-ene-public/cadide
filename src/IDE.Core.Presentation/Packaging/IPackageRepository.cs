namespace IDE.Core.Presentation.Packaging;

public interface IPackageRepository
{
    Task<IList<PackageInfo>> GetPackages(string packageSource);

    /// <summary>
    /// Download and extract package
    /// </summary>
    Task RestorePackage(PackageInfoRef packageInfoRef, string destinationFolderPath);
}

public interface IFolderPackageRepository : IPackageRepository
{

}

public interface IGithubPackageRepository : IPackageRepository
{

}
