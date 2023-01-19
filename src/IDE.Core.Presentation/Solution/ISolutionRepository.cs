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
        ProjectDocument LoadProjectDocument(string filePath);

        void SaveProjectDocument(ProjectDocument project, string filePath);

        IList<ProjectInfo> GetSolutionProjects(string solutionFilePath);

        IList<string> GetProjectsFromSolution(string solutionFilePath);

        void SolutionRemoveProject(string solutionFilePath, string projectName);

        void RenameProject(string solutionFilePath, string oldFilePath, string newFilePath);

        Task<IFileBaseViewModel> OpenDocumentAsync(string filePath);

        Task<LibraryItem> LoadLibraryItemAsync(string filePath);

        SolutionProjectItem AddProjectToSolution(string solutionFilePath, string projectFilePath);

        void ProjectSetNewReferences(string projectFilePath, IList<IProjectDocumentReference> references);

        void ProjectRemoveReference(string projectFilePath, string referenceName);

        string GetProjectFilePath(string filePath);

        string GetSolutionFilePath(string filePath);
    }
}
