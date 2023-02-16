using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection.Metadata;
using IDE.Core.ViewModels;

namespace IDE.Core.Presentation.AutoUpdates;

public class UpdatesManager : IUpdatesManager
{
    private readonly IReleasesManager _releasesManager;
    private const bool includePrereleases = true;

    public UpdatesManager(IReleasesManager releasesManager)
    {
        _releasesManager = releasesManager;
    }
    public async Task StartUpdate()
    {
        var latestRelease = await CheckUpdates();
        if (latestRelease == null)
        {
            throw new Exception("There are no latest updates");
        }

        var downloadPath = await _releasesManager.DownloadRelease(latestRelease.ReleaseAssetDownloadUrl, latestRelease.AssetFileName);

        var executablePath = Process.GetCurrentProcess().MainModule?.FileName;
        var installationFolder = Path.GetDirectoryName(executablePath);

        //copy/extract installer from zip to temp
        var installerPath = SetupInstaller(downloadPath, installationFolder);

        //run installer
        var processStartInfo = new ProcessStartInfo
        {
            FileName = installerPath,
            UseShellExecute = true,
            Arguments = $"--input \"{downloadPath}\" --output \"{installationFolder}\" --executable \"{executablePath}\"  --clear",
            //Verb = "runas"    // run as admin
        };
        Process.Start(processStartInfo);

        //exit app
        Exit();
    }


    private string SetupInstaller(string downloadPath, string installationFolder)
    {
        var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        ZipFile.ExtractToDirectory(downloadPath, folder, true);

        var installerPath = Path.Combine(folder, "InstallerApp.exe");
        if (!File.Exists(installerPath))
        {
            //copy from already existing app
            var sourceInstaller = Path.Combine(installationFolder, "InstallerApp.exe");
            if(File.Exists(sourceInstaller))
            {
                File.Copy(sourceInstaller, installerPath);
            }
        }

        return installerPath;
    }

    private void Exit()
    {
        var currentProcess = Process.GetCurrentProcess();

        //close the other processes
        foreach (var process in Process.GetProcessesByName(currentProcess.ProcessName))
        {
            string processPath;
            try
            {
                processPath = process.MainModule?.FileName;
            }
            catch
            {
                continue;
            }

            //get all instances of assembly except current
            if (process.Id != currentProcess.Id && currentProcess.MainModule?.FileName == processPath)
            {
                if (process.CloseMainWindow())
                {
                    process.WaitForExit(TimeSpan.FromSeconds(10));
                }

                if (!process.HasExited)
                {
                    process.Kill(); //TODO show UI message asking user to close program himself instead of silently killing it
                }
            }
        }

        //close this process
        Environment.Exit(0);
    }

    public async Task<ReleaseInfo> CheckUpdates()
    {
        var latestRelease = await _releasesManager.GetLatestRelease(AppHelpers.ApplicationVersionMajor, includePrereleases);

        if (latestRelease != null)
        {
            var latestVersionString = latestRelease.Version.Split('-')[0];
            latestVersionString = latestVersionString.Replace("v", "").Trim();

            var latestVersion = new Version(latestVersionString);
            var thisAppVersion = new Version(AppHelpers.ApplicationVersion);

            if (latestVersion > thisAppVersion)
            {
                return latestRelease;
            }
        }

        return null;
    }
}

