using System.IO;

namespace IDE.Core.Designers;

public class FileSystemHelper
{
    public static IList<string> GetFilesWithExtension(string folderPath, string[] extensions)
    {
        return Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                        .Where(file => extensions.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
    }
}
