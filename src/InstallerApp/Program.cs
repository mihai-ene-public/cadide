using System.Diagnostics;
using System.IO.Compression;

namespace InstallerApp;

internal class Program
{
    static async Task Main(string[] args)
    {
#pragma warning disable CA1416 // Validate platform compatibility
        Console.SetWindowSize(85, 43);
#pragma warning restore CA1416 // Validate platform compatibility

        string? zipPath = null;
        string? installationFolderPath = null;
        string? exeFilePath = null;

        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index].ToLower();
            switch (arg)
            {
                case "--input":
                    zipPath = args[index + 1];
                    break;
                case "--output":
                    installationFolderPath = args[index + 1];
                    break;
                case "--executable":
                    exeFilePath = args[index + 1];
                    break;
            }
        }
        if (string.IsNullOrEmpty(zipPath))
        {
            Console.WriteLine("Zip file path (input parameter) not specified");
            return;
        }
        if (string.IsNullOrEmpty(installationFolderPath))
        {
            Console.WriteLine("Installation folder path (output parameter) not specified");
            return;
        }
        if (string.IsNullOrEmpty(exeFilePath))
        {
            Console.WriteLine("Exe path (executable parameter) not specified");
            return;
        }

        await DeployUpdate(zipPath, installationFolderPath, exeFilePath);

        Console.WriteLine("Done.");
        await Task.Delay(3000);
    }

    private static async Task DeployUpdate(string zipPath, string installationFolderPath, string exeFilePath)
    {
        await Task.CompletedTask;

        if (string.IsNullOrEmpty(zipPath)
            || string.IsNullOrEmpty(installationFolderPath)
            || string.IsNullOrEmpty(exeFilePath))
        {
            return;
        }

        foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exeFilePath)))
        {
            try
            {
                if (process.MainModule?.FileName == exeFilePath)
                {
                    Console.WriteLine("Waiting for application to exit...");
                    process.WaitForExit();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        //Console.WriteLine("Pausing...");
        await Task.Delay(1000);

        Console.WriteLine("Extracting...");
        ZipFile.ExtractToDirectory(zipPath, installationFolderPath, true);


    }

}
