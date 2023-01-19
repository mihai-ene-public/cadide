using IDE.Core.Common;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.ViewModels;

public class ProjectFolderNodeModel : FilesContainerNodeModel, IProjectFolderNodeModel
{

    public ProjectFolderNodeModel()
    {
        IsReadOnly = false;
        IsExpanded = false;
    }

    public override void Load(string filePath)
    {
        base.Load(filePath);

        LoadFolder(filePath);
    }
}
