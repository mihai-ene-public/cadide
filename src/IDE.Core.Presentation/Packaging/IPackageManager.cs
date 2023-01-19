namespace IDE.Core.Presentation.Packaging;

public interface IPackageManager
{
    Task<IList<PackageInfo>> GetPackages();

    Task<IList<PackageInfoRef>> GetProjectInstalledPackages(string projectPath);

    Task ProjectInstallPackage(string projectPath, PackageInfoRef packageInfoRef);

    Task ProjectUninstallPackage(string projectPath, PackageInfoRef packageInfoRef);

    Task ProjectUpdatePackage(string projectPath, PackageInfoRef packageInfoRef);

    Task RestorePackage(PackageInfoRef packageInfoRef);

    Task RestorePackages(string projectOrSolutionPath);
}
