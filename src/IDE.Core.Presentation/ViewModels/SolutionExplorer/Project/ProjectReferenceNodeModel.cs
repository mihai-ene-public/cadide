using System.IO;
using IDE.Core.Storage;

namespace IDE.Core.ViewModels
{
    public class ProjectReferenceNodeModel : SolutionExplorerNodeModel
    {
        protected override string GetNameInternal()
        {
            //return Path.GetFileNameWithoutExtension(FileName);
            return FileName;
        }

    }
}
