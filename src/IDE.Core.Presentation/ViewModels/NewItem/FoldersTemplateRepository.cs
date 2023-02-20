using IDE.Core;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System.IO;

namespace IDE.Documents.Views;

public class FoldersTemplateRepository : IFoldersTemplateRepository
{
    public void CreateItemFromTemplate(string templateFilePath, string itemFilePath, bool isProjectTemplate)
    {
        //copy the files (except .template) from template folder to Location
        //bug: for more files, copies files with the same name as item name from window
        var templateFolder = Path.GetDirectoryName(templateFilePath);
        var templateExtension = Path.GetExtension(templateFilePath);
        foreach (var templateFile in Directory.GetFiles(templateFolder))
        {
            var templateFileExtension = Path.GetExtension(templateFile);
            if (templateFileExtension != templateExtension)
            {
                var destFile = itemFilePath + templateFileExtension;
                if (isProjectTemplate && Path.GetExtension(destFile) != ProjectDocument.ProjectExtension)
                {
                    //we just copy the same file with the same extension
                    var fn = Path.GetFileName(templateFile);
                    var destFolder = Path.GetDirectoryName(itemFilePath);

                    destFile = Path.Combine(destFolder, fn);
                }
                File.Copy(templateFile, destFile);
            }
        }

        //copy folders recusively
        var destProjFolder = Path.GetDirectoryName(itemFilePath);
        foreach (var templateSubFolder in Directory.GetDirectories(templateFolder))
        {
            var destFolder = Path.Combine(destProjFolder, Path.GetFileName(templateSubFolder));

            Extensions.CopyDirectory(templateSubFolder, destFolder, false);
        }
    }

    public IList<TemplateItemInfo> LoadTemplates(TemplateType templateType)
    {
        var folders = new[] { "Templates" };
        IList<TemplateItemInfo> templates = new List<TemplateItemInfo>();

        switch (templateType)
        {
            case TemplateType.Solution:
            case TemplateType.Project:
            case TemplateType.SampleProject:
                {
                    templates = LoadGroupsFromFolders(folders, "Projects", templateType);
                    break;
                }

            case TemplateType.Symbol:
            case TemplateType.Footprint:
            case TemplateType.Model:
            case TemplateType.Component:
            case TemplateType.Schematic:
            case TemplateType.Board:
            case TemplateType.Misc:
                {
                    templates = LoadGroupsFromFolders(folders, "Items", templateType);
                    break;
                }
        }

        return templates;

    }

    private IList<TemplateItemInfo> LoadGroupsFromFolders(string[] folders, string subFolder, TemplateType templateType)
    {
        var templates = new List<TemplateItemInfo>();

        foreach (var folder in folders)
        {
            //Projects folder
            foreach (var projRootDir in Directory.GetDirectories(folder, subFolder))
            {
                foreach (var groupDir in Directory.GetDirectories(projRootDir))
                {
                    templates.AddRange(LoadTemplatesByFolder(groupDir, templateType));
                }
            }
        }

        return templates;
    }

    private IList<TemplateItemInfo> LoadTemplatesByFolder(string folderPath, TemplateType templateType)
    {
        var templates = new List<TemplateItemInfo>();
        foreach (var templateFolder in Directory.GetDirectories(folderPath))
        {
            //take the 1st template file
            var templateFile = Directory.GetFiles(templateFolder, "*.template").FirstOrDefault();
            if (!string.IsNullOrEmpty(templateFile))
            {
                //deserialize it; ignore the errors
                try
                {
                    var templateItem = XmlHelper.Load<TemplateItem>(templateFile);

                    //if we create a new solution, we add search for projects
                    var type = templateType;
                    if (type == TemplateType.Solution)// || type == TemplateType.SampleProject)
                        type = TemplateType.Project;

                    if (type == templateItem.TemplateType)
                        templates.Add(new FolderTemplateItemInfo
                        {
                            TemplateItem = templateItem,
                            TemplateFilePath = templateFile
                        });
                }
                catch { }
            }
        }

        return templates;
    }

}
