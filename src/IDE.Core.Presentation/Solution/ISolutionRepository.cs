using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Core.Presentation.Solution
{
    public interface ISolutionRepository
    {
        IList<IProjectDocument> GetSolutionProjects(string solutionFilePath);

        IList<ISolutionProjectNodeModel> GetProjectsFromSolution(string solutionFilePath);

        void LoadSolutionProjects(ISolutionRootNodeModel solution, IList<ISolutionProjectNodeModel> solutionProjects);

        Task<IFileBaseViewModel> OpenDocumentAsync(ISolutionExplorerNodeModel item);

        Task<LibraryItem> LoadLibraryItemAsync(string filePath);
    }
}
