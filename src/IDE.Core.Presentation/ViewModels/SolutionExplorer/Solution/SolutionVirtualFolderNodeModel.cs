using IDE.Core.Common;
using IDE.Core.Storage;

namespace IDE.Core.ViewModels
{
    public class SolutionVirtualFolderNodeModel : SolutionExplorerNodeModel
    {
        GroupFolderItem Folder { get { return Document as GroupFolderItem; } }

        public override void Load(string filePath)
        {
            foreach (var child in Folder.Children)
            {
                if (child is GroupFolderItem)
                {
                    var folderModel = new SolutionVirtualFolderNodeModel
                    {
                        Document = child
                    };
                    folderModel.Load(null);
                    AddChild(folderModel);
                }
                if (child is SolutionProjectItem)
                {
                    var project = child as SolutionProjectItem;
                    var projectModel = project.CreateSolutionExplorerNodeModel();
                    AddChild(projectModel);
                }
            }
        }
    }
}
