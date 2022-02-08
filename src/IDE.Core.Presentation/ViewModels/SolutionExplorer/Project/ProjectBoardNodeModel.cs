using System.IO;
using IDE.Core.Interfaces;

namespace IDE.Core.ViewModels
{
    public class ProjectBoardNodeModel : SolutionExplorerNodeModel, IProjectBoardNodeModel
    {
        public ProjectBoardNodeModel()
        {
            IsReadOnly = false;
        }
    }
}
