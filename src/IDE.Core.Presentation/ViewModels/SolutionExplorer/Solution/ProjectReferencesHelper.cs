using System.IO;
using System.Collections.Generic;
using IDE.Core.Storage;
using IDE.Core.Utilities;
using IDE.Core.Settings;
using IDE.Core.Interfaces;
using System.Linq;

namespace IDE.Core.ViewModels
{
    public class ProjectReferencesHelper
    {
        private static IList<string> GetSearchFolders(ISolutionProjectNodeModel projectNode)
        {
            var libFolders = new List<string>();

            var settingsManager = ServiceProvider.Resolve<ISettingsManager>();

            //project folder
            var projectFolder = projectNode?.GetItemFolderFullPath();
            if (!string.IsNullOrEmpty(projectFolder))
                libFolders.Add(projectFolder);

            //all folders specified in settings
            var s = settingsManager.GetSetting<EnvironmentFolderLibsSettingData>();
            if (s != null)
            {
                libFolders.AddRange(s.Folders);
            }

            return libFolders;
        }

        public static string FindLibraryFullPath(ISolutionProjectNodeModel projectNode, string hintPath)
        {
            if (Path.IsPathRooted(hintPath))
                return hintPath;

            var libFolders = GetSearchFolders(projectNode);
            var libraryName = Path.GetFileNameWithoutExtension(hintPath);
            var hintPathFolder = Path.GetDirectoryName(hintPath);

            foreach (var libFolder in libFolders)
            {
                if (!Directory.Exists(libFolder))
                    continue;

                var relativeFolder = Path.Combine(libFolder, hintPathFolder);
                string libFile = null;

                if (Directory.Exists(relativeFolder))
                {
                    //search in the relative path first
                    libFile = Directory.GetFiles(relativeFolder, $"{libraryName}.library", SearchOption.TopDirectoryOnly)
                                           .FirstOrDefault();
                    if (!string.IsNullOrEmpty(libFile))
                        return Path.GetFullPath(libFile);
                }

                //we search subfolders, in case libraries are moved
                libFile = Directory.GetFiles(libFolder, $"{libraryName}.library", SearchOption.AllDirectories)
                                       .FirstOrDefault();
                //the file exists because it is searched in a folder
                if (!string.IsNullOrEmpty(libFile))
                    return Path.GetFullPath(libFile);
            }

            return hintPath;
        }

        public void UpdateReferences(IEnumerable<ISolutionExplorerNodeModel> nodes)
        {
            var projectNode = nodes.FirstOrDefault()?.ProjectNode;
            if (projectNode == null)
                return;

            foreach (var node in nodes)
            {
                if (node.Document is LibraryProjectReference lib)
                {
                    //var projectNode = node.ProjectNode;
                    var referencesFolder = Path.Combine(projectNode.GetItemFolderFullPath(), "References");
                    Directory.CreateDirectory(referencesFolder);

                    var libRefFile = Path.Combine(projectNode.GetItemFolderFullPath(), "References", lib.LibraryName + ".libref");
                    var libSrcFile = FindLibraryFullPath(projectNode, lib.HintPath);

                    if (!string.IsNullOrEmpty(libSrcFile) && File.Exists(libSrcFile))
                    {
                        try
                        {
                            File.Copy(libSrcFile, libRefFile, true);
                        }
                        catch { }
                    }
                }

            }
        }

    }
}
