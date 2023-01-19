using System.IO;
using System.IO.Compression;
using IDE.Core.Data.Packages;
using IDE.Core.Presentation.Solution;
using IDE.Core.Storage;

namespace IDE.Core.Presentation.Builders;

public class SolutionPackageBuilder : ISolutionPackageBuilder
{
    private readonly ISolutionBuilder _solutionBuilder;
    private readonly ISolutionRepository _solutionRepository;

    public SolutionPackageBuilder(
        ISolutionBuilder solutionBuilder,
        ISolutionRepository solutionRepository
        )
    {
        _solutionBuilder = solutionBuilder;
        _solutionRepository = solutionRepository;
    }

    public async Task BuildProject(string filePath)
    {
        var files = await _solutionBuilder.BuildProject(filePath);

        files = files.Where(f => Path.GetExtension(f) == ".library").ToList();

        if (files.Count == 0)
        {
            throw new Exception($"Project is not a library project type: {Path.GetFileName(filePath)}");
        }

        //create manifest
        var project = _solutionRepository.LoadProjectDocument(filePath);
        var packInfo = project.Package;

        CreatePackageFile(packInfo, files, filePath);
    }

    public async Task BuildSolution(string solutionFilePath)
    {
        var files = await _solutionBuilder.BuildSolution(solutionFilePath);

        files = files.Where(f => Path.GetExtension(f) == ".library").ToList();

        if (files.Count == 0)
        {
            throw new Exception($"No library project type was found in solution: {Path.GetFileName(solutionFilePath)}");
        }

        //create manifest
        var solution = XmlHelper.Load<SolutionDocument>(solutionFilePath);
        var packInfo = solution.Package;

        CreatePackageFile(packInfo, files, solutionFilePath);
    }

    private void ValidatePackageInfo(PackageMetadata packInfo)
    {
        if (packInfo == null)
        {
            throw new Exception($"Package info was not specified.");
        }
        if (string.IsNullOrEmpty(packInfo.Id))
        {
            throw new Exception($"Package Id was not specified");
        }
        if (string.IsNullOrEmpty(packInfo.Version))
        {
            throw new Exception($"Package version was not specified");
        }
    }

    private void CreatePackageFile(PackageMetadata packInfo, IList<string> files, string projectOrSolutionFilePath)
    {
        var projFolder = Path.GetDirectoryName(projectOrSolutionFilePath);
        ValidatePackageInfo(packInfo);
        var packFolder = Path.GetDirectoryName(files[0]);
        var packInfoPath = Path.Combine(packFolder, $"{packInfo.Id}.info");
        XmlHelper.Save(packInfo, packInfoPath);
        files.Add(packInfoPath);

        //icon folder is the same as either solution folder or project folder
        if (!string.IsNullOrEmpty(packInfo.Icon))
        {
            var packIconPath = Path.Combine(projFolder, packInfo.Icon);
            if (File.Exists(packIconPath))
            {
                files.Add(packIconPath);
            }
        }

        //zip file
        var packagePath = Path.Combine(packFolder, $"{packInfo.Id}.{packInfo.Version}.package");
        using (var fs = new FileStream(packagePath, FileMode.Create))
        {
            using (var zipFile = new ZipArchive(fs, ZipArchiveMode.Create, false))
            {
                foreach (var file in files)
                {
                    var entryName = Path.GetExtension(file) == ".library"
                                        ? Path.Combine("lib", Path.GetFileName(file))
                                        : Path.GetFileName(file);
                    var entry = zipFile.CreateEntryFromFile(file, entryName);
                }
            }
        }
    }
}
