using IDE.Core.Interfaces;

namespace IDE.Core.ViewModels;

public class ExplorerNodeComparer : IComparer<ISolutionExplorerNodeModel>
{
    public int Compare(ISolutionExplorerNodeModel x, ISolutionExplorerNodeModel y)
    {
        if (x is FilesContainerNodeModel && y is not FilesContainerNodeModel)
        {
            return 1;
        }
        else if (x is not FilesContainerNodeModel && y is FilesContainerNodeModel)
        {
            return -1;
        }

        if (string.IsNullOrEmpty(x.Name) || string.IsNullOrEmpty(y.Name))
            return 0;

        return x.Name.CompareTo(y.Name);
    }
}

