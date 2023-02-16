using System.IO;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Solution;
using IDE.Core.Settings;
using IDE.Core.Storage;
using IDE.Core.ViewModels;

namespace IDE.Core.Presentation.Packaging;

public class PackageManager : IPackageManager
{
    private readonly IPackageSourceRepository _packageSourceRepository;
    private readonly IPackageRepositoryFactory _packageRepositoryFactory;
    private readonly ISolutionRepository _solutionRepository;
    private readonly ISettingsManager _settingsManager;

    public PackageManager(
        IPackageSourceRepository packageSourceRepository,
        IPackageRepositoryFactory packageRepositoryFactory,
        ISolutionRepository solutionRepository,
        ISettingsManager settingsManager
        )
    {
        _packageSourceRepository = packageSourceRepository;
        _packageRepositoryFactory = packageRepositoryFactory;
        _solutionRepository = solutionRepository;
        _settingsManager = settingsManager;
    }

    public async Task<IList<PackageInfo>> GetPackages()
    {
        await Task.CompletedTask;

        var packages = new List<PackageInfo>();

        var packageSources = _packageSourceRepository.GetPackageSources();

        foreach (var packageSource in packageSources)
        {
            if (!packageSource.IsEnabled)
                continue;

            var packRepo = _packageRepositoryFactory.Create(packageSource.Source);
            if (packRepo == null)
            {
                continue;
            }

            var sourcePackages = await packRepo.GetPackages(packageSource.Source);
            packages.AddRange(sourcePackages);
        }

        return packages;
    }

    public async Task<IList<PackageInfoRef>> GetProjectInstalledPackages(string projectPath)
    {
        await Task.CompletedTask;
        var project = _solutionRepository.LoadProjectDocument(projectPath);

        var packages = project.References.OfType<PackageProjectReference>()
            .Select(r => new PackageInfoRef
            {
                PackageId = r.PackageId,
                PackageSource = r.PackageSource,
                PackageVersion = r.PackageVersion
            })
            .ToList();

        return packages;
    }

    public Task ProjectInstallPackage(string projectPath, PackageInfoRef packageInfoRef)
    {
        var project = _solutionRepository.LoadProjectDocument(projectPath);

        var existingPackage = project.References.OfType<PackageProjectReference>()
                                     .FirstOrDefault(p => p.PackageId == packageInfoRef.PackageId);

        if (existingPackage == null)
        {
            project.References.Add(new PackageProjectReference
            {
                PackageId = packageInfoRef.PackageId,
                PackageSource = packageInfoRef.PackageSource,
                PackageVersion = packageInfoRef.PackageVersion
            });

            _solutionRepository.SaveProjectDocument(project, projectPath);
        }

        return Task.CompletedTask;
    }

    public Task ProjectUninstallPackage(string projectPath, PackageInfoRef packageInfoRef)
    {
        var project = _solutionRepository.LoadProjectDocument(projectPath);

        var existingPackage = project.References.OfType<PackageProjectReference>()
                                     .FirstOrDefault(p => p.PackageId == packageInfoRef.PackageId);


        if (existingPackage != null)
        {
            project.References.Remove(existingPackage);

            _solutionRepository.SaveProjectDocument(project, projectPath);
        }

        return Task.CompletedTask;
    }

    public Task ProjectUpdatePackage(string projectPath, PackageInfoRef packageInfoRef)
    {
        var project = _solutionRepository.LoadProjectDocument(projectPath);

        var existingPackage = project.References.OfType<PackageProjectReference>()
                                     .FirstOrDefault(p => p.PackageId == packageInfoRef.PackageId);


        if (existingPackage != null)
        {
            existingPackage.PackageVersion = packageInfoRef.PackageVersion;
            existingPackage.PackageSource = packageInfoRef.PackageSource;

            _solutionRepository.SaveProjectDocument(project, projectPath);
        }

        return Task.CompletedTask;
    }

    public async Task RestorePackage(PackageInfoRef packageInfoRef)
    {
        var folderPath = GetPackagesCacheFolderPath();
        if (string.IsNullOrEmpty(folderPath))
            return;

        var packagesCacheFolderPath = Environment.ExpandEnvironmentVariables(folderPath);
        var packRepo = _packageRepositoryFactory.Create(packageInfoRef.PackageSource);

        if (packRepo == null)
        {
            throw new Exception($"No repository repo registered for package source: '{packageInfoRef.PackageSource}'");
        }

        await packRepo.RestorePackage(packageInfoRef, packagesCacheFolderPath);
    }

    public async Task RestorePackages(string projectOrSolutionPath)
    {
        var packRefs = GetPackRefs(projectOrSolutionPath);

        foreach (var packRef in packRefs)
        {
            await RestorePackage(packRef);
        }
    }

    private IList<PackageInfoRef> GetPackRefs(string projectOrSolutionPath)
    {
        var packRefs = new List<PackageInfoRef>();

        var fileExtension = Path.GetExtension(projectOrSolutionPath);
        if (fileExtension.StartsWith("."))
            fileExtension = fileExtension.Substring(1);

        switch (fileExtension)
        {
            case "solution":
                {
                    var projects = _solutionRepository.GetSolutionProjects(projectOrSolutionPath);
                    foreach (var project in projects)
                    {
                        packRefs.AddRange(project.Project.References.OfType<PackageProjectReference>().Select(p =>
                        new PackageInfoRef
                        {
                            PackageId = p.PackageId,
                            PackageVersion = p.PackageVersion,
                            PackageSource = p.PackageSource,
                        }));
                    }
                    break;
                }

            case "project":
                {
                    var project = _solutionRepository.LoadProjectDocument(projectOrSolutionPath);
                    packRefs.AddRange(project.References.OfType<PackageProjectReference>().Select(p =>
                       new PackageInfoRef
                       {
                           PackageId = p.PackageId,
                           PackageVersion = p.PackageVersion,
                           PackageSource = p.PackageSource,
                       }));
                    break;
                }

            default:
                throw new Exception("This file type is not supported for building. Only solution file types allowed.");
        }

        return packRefs;
    }
    private string GetPackagesCacheFolderPath()
    {
        var packageSetting = _settingsManager.GetSetting<PackageManagerSettings>();
        return packageSetting.PackagesCacheFolderPath;
    }
}
