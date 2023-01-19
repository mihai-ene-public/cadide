using System.IO;
using IDE.Core.Interfaces;
using IDE.Core.Settings;

namespace IDE.Core.Presentation.ObjectFinding;

public class LibraryRepository : ILibraryRepository
{
    private readonly ISettingsManager _settingsManager;

    public LibraryRepository(ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public string FindLibraryFilePath(string libraryName)
    {
        var foldersSettings = _settingsManager.GetSetting<EnvironmentFolderLibsSettingData>();

        if (foldersSettings != null)
        {
            foreach (var folder in foldersSettings.Folders)
            {
                var libFolder = Environment.ExpandEnvironmentVariables(folder);
                var libFiles = Directory.GetFiles(libFolder, $"{libraryName}.library", SearchOption.AllDirectories);

                if (libFiles.Length > 0)
                    return libFiles[0];
            }
        }

        return null;
    }

    public IList<string> GetAllLibrariesFilePaths()
    {
        var libPaths = new List<string>();

        var foldersSettings = _settingsManager.GetSetting<EnvironmentFolderLibsSettingData>();

        if (foldersSettings != null)
        {
            foreach (var folder in foldersSettings.Folders)
            {
                var libFolder = Environment.ExpandEnvironmentVariables(folder);

                libPaths.AddRange(Directory.GetFiles(libFolder, "*.library", SearchOption.AllDirectories));
            }
        }

        return libPaths;
    }

    public IList<string> GetAllLibrariesFilePaths(string packageId, string packageVersion)
    {
        var libPaths = new List<string>();

        var packagesCacheFolderPath = Environment.ExpandEnvironmentVariables(GetPackagesCacheFolderPath());
        var packageFolder = Path.Combine(packagesCacheFolderPath, packageId, packageVersion, "lib");

        if (Directory.Exists(packageFolder))
        {
            libPaths.AddRange(Directory.GetFiles(packageFolder, "*.library", SearchOption.TopDirectoryOnly));
        }

        return libPaths;
    }


    private string GetPackagesCacheFolderPath()
    {
        var packageSetting = _settingsManager.GetSetting<PackageManagerSettings>();
        return packageSetting.PackagesCacheFolderPath;
    }
}
