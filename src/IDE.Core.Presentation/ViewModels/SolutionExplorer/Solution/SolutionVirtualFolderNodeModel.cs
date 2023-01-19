using IDE.Core.Common;
using IDE.Core.Storage;

namespace IDE.Core.ViewModels
{
    public class SolutionVirtualFolderNodeModel : SolutionExplorerNodeModel
    {

        public override void Load(string filePath)
        {
            /*virtual folders are not implemented yet
            foreach (var child in Folder.Children)
            {
                if (child is GroupFolderItem)
                {
                    var folderModel = new SolutionVirtualFolderNodeModel();
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
            */
        }
    }
}
