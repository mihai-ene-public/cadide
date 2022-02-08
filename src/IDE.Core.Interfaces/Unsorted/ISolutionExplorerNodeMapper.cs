using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface ISolutionExplorerNodeMapper : IService
    {
        ISolutionExplorerNodeModel CreateSolutionExplorerNodeModel(IProjectFileRef fileItem);
    }

    public interface IFileExtensionToSolutionExplorerNodeMapper : IService
    {
        ISolutionExplorerNodeModel CreateSolutionExplorerNodeModel(string extension);
    }
}
