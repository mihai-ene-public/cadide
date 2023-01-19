using IDE.Core.Interfaces;
using System.IO;

namespace IDE.Core.ViewModels;

/// <summary>
/// represents a node that contains multiple files and folders
/// <para>A project node and a Folder node should inherit from this</para>
/// </summary>
public class FilesContainerNodeModel : SolutionExplorerNodeModel
{
    protected void LoadFolder(string folderPath)
    {
        //folders
        var folders = new List<ISolutionExplorerNodeModel>();
        var excludedFolders = new[] { "!Output", "References" };
        foreach (var folder in Directory.GetDirectories(folderPath))
        {
            var folderName = Path.GetFileName(folder);
            if (excludedFolders.Contains(folderName))
                continue;
            var f = CreateSolutionExplorerFolderNodeModel(folder);
            folders.Add(f);
        }
        AddChildren(folders);

        var files = new List<ISolutionExplorerNodeModel>();
        var excludedFileExtensions = new[] { "project" };
        foreach (var file in Directory.GetFiles(folderPath))
        {
            var ext = Path.GetExtension(file);
            ext = ext.Replace(".", "");
            if (excludedFileExtensions.Contains(ext))
                continue;
            var f = CreateSolutionExplorerNodeModel(ext);
            files.Add(f);
            f.Load(file);
        }

        AddChildren(files);
    }
}
