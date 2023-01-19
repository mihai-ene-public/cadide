using System.IO;

namespace IDE.Core;



public static class PathHelper
{
    public static bool IsDirectory(string path)
    {
        var attr = File.GetAttributes(path);
        return attr.HasFlag(FileAttributes.Directory);
    }

    public static bool FolderPathContainsFile(string folderPath, string filePath)
    {
        var fileFolder = Path.GetDirectoryName(filePath);
        while (!string.IsNullOrEmpty(fileFolder))
        {
            if (folderPath == fileFolder)
                return true;

            fileFolder = Path.GetDirectoryName(fileFolder);
        }

        return false;
    }
}
