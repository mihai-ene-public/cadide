using System;
using System.Collections.Generic;
using System.IO;
using IDE.Core.Common;
using IDE.Core.Documents;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.ViewModels;

namespace IDE.Core.Presentation.Solution
{
    public class SolutionRepository : ISolutionRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDocumentTypeManager _documentTypeManager;

        public SolutionRepository(IServiceProvider serviceProvider, IDocumentTypeManager documentTypeManager)
        {
            _serviceProvider = serviceProvider;
            _documentTypeManager = documentTypeManager;
        }

        public IList<IProjectDocument> GetSolutionProjects(string solutionFilePath)
        {
            var solution = SolutionManager.LoadSolution(solutionFilePath);

            var projects = new List<IProjectDocument>();

            foreach (var child in solution.Children)
            {
                if (child is SolutionProjectItem projectItem)
                {
                    var projectPath = Path.Combine(Path.GetDirectoryName(SolutionManager.SolutionFilePath), projectItem.RelativePath);
                    var projDoc = ProjectDocument.Load(projectPath.Replace(@"/", @"\"));
                    projects.Add(projDoc);
                }
            }

            return projects;
        }

        public IList<ISolutionProjectNodeModel> GetProjectsFromSolution(string solutionFilePath)
        {
            var solutionProjects = new List<ISolutionProjectNodeModel>();

            var solutionDoc = SolutionManager.LoadSolution(solutionFilePath);
            ISolutionRootNodeModel solution = new SolutionRootNodeModel();
            solution.Document = solutionDoc;

            LoadSolutionProjects(solution, solutionProjects);

            return solutionProjects;
        }

        public void LoadSolutionProjects(ISolutionRootNodeModel solution, IList<ISolutionProjectNodeModel> solutionProjects)
        {
            var projects = new List<SolutionProjectItem>();
            LoadProjectListLinear(solution.Solution.Children, projects);

            foreach (var project in projects)
            {
                var projectModel = (ISolutionProjectNodeModel)project.CreateSolutionExplorerNodeModel();

                if (projectModel == null)
                    throw new Exception("var projectModel = CreateSolutionExplorerNodeModel(p);");

                projectModel.ParentNode = solution;
                solutionProjects.Add(projectModel);
            }
        }

        public async Task<IFileBaseViewModel> OpenDocumentAsync(ISolutionExplorerNodeModel item)
        {
            IFileBaseViewModel fileViewModel = null;
            var filePath = item.GetItemFullPath();

            var fileExtension = Path.GetExtension(filePath);

            var docType = _documentTypeManager.FindDocumentTypeByExtension(fileExtension);

            if (docType == null)
                throw new Exception($"Could not find a file opener for this filetype: {fileExtension}");

            fileViewModel = _serviceProvider.GetService(docType.DocumentEditorClassType) as IFileBaseViewModel;

            if (fileViewModel == null)
                throw new Exception($"Could not find a file opener for this filetype: {fileExtension}");

            fileViewModel.LoadedForCompiler = true;
            fileViewModel.Item = item;
            await fileViewModel.OpenFileAsync(filePath);

            return fileViewModel;
        }

        public async Task<LibraryItem> LoadLibraryItemAsync(string filePath)
        {
            await Task.CompletedTask;

            var fileExtension = Path.GetExtension(filePath);

            var docType = _documentTypeManager.FindDocumentTypeByExtension(fileExtension);

            if (docType != null && docType.DocumentClassType != null)
            {
                var libItem = XmlHelper.Load(filePath, docType.DocumentClassType);

                return (LibraryItem)libItem;
            }

            return null;
        }


        private void LoadProjectListLinear(IList<ProjectBaseFileRef> children, IList<SolutionProjectItem> projects)
        {
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child is SolutionProjectItem project)
                        projects.Add(project);
                    else if (child is GroupFolderItem folder)
                        LoadProjectListLinear(folder.Children, projects);
                }
            }
        }

    }
}
