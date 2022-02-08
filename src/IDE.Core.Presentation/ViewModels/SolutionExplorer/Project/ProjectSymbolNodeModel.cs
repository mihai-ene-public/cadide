using System.IO;

namespace IDE.Core.ViewModels
{
    public class ProjectSymbolNodeModel : SolutionExplorerNodeModel
    {


        public ProjectSymbolNodeModel()
        {
            IsReadOnly = false;
        }

    }

    public class ProjectFontNodeModel : SolutionExplorerNodeModel
    {
        public ProjectFontNodeModel()
        {
            IsReadOnly = false;
        }
    }
}
