using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection.Metadata;
using System.Xml.Serialization;
using IDE.Core.Common;
using IDE.Core.Common.FileSystem;
using IDE.Core.Documents;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.ViewModels;

namespace IDE.Core.Presentation.Solution;

/*
 * Assumming this file folder structure
 * Solution folder
 *      solutionFile.solution
 *      ProjectFolder1
 *          ProjectFile.project
 *          files
 *          Folder1
 *              files in Folder1
 * 
 */
public class SolutionRepository : ISolutionRepository
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDocumentTypeManager _documentTypeManager;
    private readonly IFileSystemProvider _fileSystemProvider;

    public SolutionRepository(
        IServiceProvider serviceProvider,
        IDocumentTypeManager documentTypeManager,
        IFileSystemProvider fileSystemProvider
        )
    {
        _serviceProvider = serviceProvider;
        _documentTypeManager = documentTypeManager;
        _fileSystemProvider = fileSystemProvider;
    }

    private SolutionDocument LoadSolution(string solutionFilePath)
    {
        var solution = XmlHelper.Load<SolutionDocument>(solutionFilePath);

        return solution;
    }

    private void SaveSolution(SolutionDocument solution, string solutionFilePath)
    {
        XmlHelper.Save(solution, solutionFilePath);
    }

    /// <summary>
    /// returns the full path of the solution this file belongs to
    /// </summary>
    public string GetSolutionFilePath(string filePath)
    {
        var path = filePath;
        if (!PathHelper.IsDirectory(filePath))
        {
            path = Path.GetDirectoryName(filePath);
        }
        var fileExtension = ".solution";

        return FindFileByExtension(path, fileExtension);
    }

    private string FindFileByExtension(string path, string extension)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var files = Directory.GetFiles(path, $"*{extension}", SearchOption.TopDirectoryOnly);

        if (files.Length > 0)
            return files[0];

        return FindFileByExtension(Path.GetDirectoryName(path), extension);
    }

    public string GetSolutionFolderPath(string filePath)
    {
        var projectPath = GetSolutionFilePath(filePath);
        return Path.GetDirectoryName(projectPath);
    }

    /// <summary>
    /// returns the full path of the project this file belongs to
    /// </summary>
    public string GetProjectFilePath(string filePath)
    {
        var path = filePath;
        if (!PathHelper.IsDirectory(filePath))
        {
            path = Path.GetDirectoryName(filePath);
        }
        var fileExtension = ".project";

        return FindFileByExtension(path, fileExtension);
    }

    /// <summary>
    /// returns the full project folder path this file belongs to
    /// </summary>
    public string GetProjectFolderPath(string filePath)
    {
        var projectPath = GetProjectFilePath(filePath);
        return Path.GetDirectoryName(projectPath);
    }

    public IList<ProjectInfo> GetSolutionProjects(string solutionFilePath)
    {
        var solution = LoadSolution(solutionFilePath);

        var projects = new List<ProjectInfo>();

        foreach (var child in solution.Children)
        {
            if (child is SolutionProjectItem projectItem)
            {
                var projectPath = Path.Combine(Path.GetDirectoryName(solutionFilePath), projectItem.RelativePath);
                var projDoc = LoadProjectDocument(projectPath.Replace(@"/", @"\"));
                projects.Add(new ProjectInfo
                {
                    Project = projDoc,
                    ProjectPath = projectPath,
                });
            }
        }

        return projects;
    }

    public ProjectDocument LoadProjectDocument(string filePath)
    {
        var project = XmlHelper.Load<ProjectDocument>(filePath);

        return project;
    }

    public void SaveProjectDocument(ProjectDocument project, string filePath)
    {
        XmlHelper.Save(project, filePath);
    }

    public SolutionProjectItem AddProjectToSolution(string solutionFilePath, string projectFilePath)
    {
        var solution = LoadSolution(solutionFilePath);

        //we add the new project to the solution
        var solFolder = Path.GetDirectoryName(solutionFilePath);

        //todo: add to virtual folder
        var projItem = new SolutionProjectItem
        {
            RelativePath = DirectoryName.GetRelativePath(solFolder, projectFilePath)
        };
        solution.Children.Add(projItem);

        SaveSolution(solution, solutionFilePath);

        return projItem;
    }

    public void ProjectSetNewReferences(string projectFilePath, IList<IProjectDocumentReference> references)
    {
        var project = LoadProjectDocument(projectFilePath);

        //the new references to the project
        project.References = references.Cast<ProjectDocumentReference>().ToList();
        SaveProjectDocument(project, projectFilePath);
    }

    public void ProjectRemoveReference(string projectFilePath, string referenceName)
    {
        var projectDoc = LoadProjectDocument(projectFilePath);
        var projFolder = Path.GetDirectoryName(projectFilePath);

        var projReference = projectDoc.References.FirstOrDefault(r => ( r is ProjectProjectReference pr && Path.GetFileNameWithoutExtension(pr.ProjectPath) == referenceName )
        || ( r is LibraryProjectReference lr && lr.LibraryName == referenceName ));

        if (projReference != null)
        {
            projectDoc.References.Remove(projReference);

            SaveProjectDocument(projectDoc, projectFilePath);
        }
    }

    public IList<SolutionProjectItem> GetSolutionProjectItems(string solutionFilePath)
    {
        var solution = LoadSolution(solutionFilePath);

        return solution.Children.OfType<SolutionProjectItem>().ToList();
    }

    public IList<string> GetProjectsFromSolution(string solutionFilePath)
    {
        var solutionProjects = new List<string>();
        var solutionFolder = Path.GetDirectoryName(solutionFilePath);

        var solution = LoadSolution(solutionFilePath);

        var projectItems = solution.Children.OfType<SolutionProjectItem>().ToList();

        foreach (var projectItem in projectItems)
        {
            var projectFilePath = Path.Combine(solutionFolder, projectItem.RelativePath);
            if (File.Exists(projectFilePath))
            {
                solutionProjects.Add(projectFilePath);
            }
        }

        return solutionProjects;
    }

    public void SolutionRemoveProject(string solutionFilePath, string projectName)
    {
        //remove the project from the solution, but don't delete any files
        var solution = LoadSolution(solutionFilePath);
        var project = solution.Children.OfType<SolutionProjectItem>().FirstOrDefault(p => Path.GetFileNameWithoutExtension(p.RelativePath) == projectName);

        if (project != null)
        {
            solution.Children.Remove(project);
            SaveSolution(solution, solutionFilePath);
        }
    }

    public void RenameProject(string solutionFilePath, string oldFilePath, string newFilePath)
    {
        var solution = LoadSolution(solutionFilePath);
        var solFolder = Path.GetDirectoryName(solutionFilePath);

        var project = solution.Children.OfType<SolutionProjectItem>()
                                       .FirstOrDefault(p => Path.GetFileName(p.RelativePath) == Path.GetFileName(oldFilePath));

        if (project == null)
            throw new Exception($"Project '{Path.GetFileName(oldFilePath)}' not found in solution '{Path.GetFileName(solutionFilePath)}'");

        project.RelativePath = DirectoryName.GetRelativePath(solFolder, newFilePath);

        SaveSolution(solution, solutionFilePath);
    }

    public async Task<IFileBaseViewModel> OpenDocumentAsync(string filePath)
    {
        var fileExtension = Path.GetExtension(filePath);

        var docType = _documentTypeManager.FindDocumentTypeByExtension(fileExtension);

        if (docType == null)
            throw new Exception($"Could not find a file opener for this filetype: {fileExtension}");

        var fileViewModel = _serviceProvider.GetService(docType.DocumentEditorClassType) as IFileBaseViewModel;

        if (fileViewModel == null)
            throw new Exception($"Could not find a file opener for this filetype: {fileExtension}");

        fileViewModel.LoadedForCompiler = true;
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


