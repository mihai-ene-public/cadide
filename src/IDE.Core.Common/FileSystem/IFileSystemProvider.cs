using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Common.FileSystem;
public interface IFileSystemProvider
{
}

public class FileSystemProvider : IFileSystemProvider
{
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public void FileDelete(string path)
    {
        File.Delete(path);
    }

    public string[] DirectoryGetFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return Directory.GetFiles(path, searchPattern, searchOption);
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public DirectoryInfo CreateDirectory(string path)
    {
        return Directory.CreateDirectory(path);
    }
}
