using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Common;
using IDE.Core.Common.Utilities;
using IDE.Core.Documents;
using IDE.Core.Errors;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Builders;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Presentation.Solution;
using IDE.Core.Storage;
using IDE.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Presentation.Compilers;

public class SolutionCompiler : ISolutionCompiler
{
    public SolutionCompiler(IFileCompiler fileCompiler, ISolutionRepository solutionRepository)
    {
        _fileCompiler = fileCompiler;
        _solutionRepository = solutionRepository;
    }

    private readonly IFileCompiler _fileCompiler;
    private readonly ISolutionRepository _solutionRepository;


    /*
    //void LoadSolution(string solutionFilePath)
    //{

    //    //load solution
    //    var solutionDoc = XmlHelper.Load<SolutionDocument>(solutionFilePath);
    //    var solution = TypeActivator.CreateInstanceByTypeName<ISolutionRootNodeModel>("SolutionRootNodeModel");

    //    if (solution == null)
    //        throw new Exception("Could not create instance of 'SolutionRootNodeModel'");

    //    solution.Document = solutionDoc;

    //    LoadSolutionProjects(solution);
    //}
    */

    /*
    void LoadSolutionProjects(ISolutionRootNodeModel solution)
    {
        solutionProjects.Clear();

        //load projects linear
        var projects = new List<SolutionProjectItem>();
        LoadProjectListLinear(solution.Solution.Children, projects);
        foreach (var p in projects)
        {
            var projectModel = (ISolutionProjectNodeModel)p.CreateSolutionExplorerNodeModel();

            if (projectModel == null)
                throw new Exception("var projectModel = CreateSolutionExplorerNodeModel(p);");
            
            projectModel.ParentNode = solution;
            solutionProjects.Add(projectModel);
        }
    }
    */

    /*
    void LoadProjectListLinear(IList<ProjectBaseFileRef> children, List<SolutionProjectItem> projects)
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
    */

    List<ISolutionProjectNodeModel> CreateBuildOrder(IList<ISolutionProjectNodeModel> solutionProjects)
    {
        //first libraries, then gerbers
        var orderedProjects = solutionProjects.OrderBy(p => p.Project.OutputType).ToList();

        return orderedProjects;
    }

    void CreateProjectItemsLinearList(List<ISolutionExplorerNodeModel> children, List<ISolutionExplorerNodeModel> libraryItems)
    {
        if (children != null)
        {
            foreach (var child in children)
            {
                if (child is IProjectReferencesNodeModel)
                    continue;

                if (child is IProjectFolderNodeModel)
                    CreateProjectItemsLinearList(( child as IProjectFolderNodeModel ).Children.ToList(), libraryItems);
                else
                    libraryItems.Add(child);
            }
        }
    }


    public async Task<bool> CompileSolution(ISolutionRootNodeModel solution)
    {
        var solutionProjects = new List<ISolutionProjectNodeModel>();
        _solutionRepository.LoadSolutionProjects(solution, solutionProjects);

        //create build order
        var orderedProjects = CreateBuildOrder(solutionProjects);

        var compileResults = new List<bool>();
        //foreach project: project.Build
        foreach (var project in orderedProjects)
        {
            var res = await CompileProject(project);
            compileResults.Add(res);
        }

        return !compileResults.Any(c => c == false);
    }

    public async Task<bool> CompileSolution(string solutionFilePath)
    {
        var solutionProjects = _solutionRepository.GetProjectsFromSolution(solutionFilePath);

        //create build order
        var orderedProjects = CreateBuildOrder(solutionProjects);

        var compileResults = new List<bool>();
        foreach (var project in orderedProjects)
        {
            var res = await CompileProject(project);
            compileResults.Add(res);
        }

        return !compileResults.Any(c => c == false);
    }

    public async Task<bool> CompileProject(ISolutionProjectNodeModel project)
    {
        SendOutputMessage($"Compiling project {project.Name}");

        //create a linear list;
        var solNodes = new List<ISolutionExplorerNodeModel>();
        var compileResults = new List<CompilerResult>();

        CreateProjectItemsLinearList(project.Children.ToList(), solNodes);


        foreach (var slnNode in solNodes)
        {
            try
            {
                var filePath = slnNode.GetItemFullPath();
                var fileExtension = Path.GetExtension(filePath);
                fileExtension = fileExtension.Replace(".", "");

                var supportedFileExtensions = new[]
                {
                    DocumentFileExtensionsConstants.BoardLongExtension,
                    DocumentFileExtensionsConstants.SchematicLongExtension,
                    DocumentFileExtensionsConstants.ComponentLongExtension,
                    DocumentFileExtensionsConstants.FootprintLongExtension,
                    DocumentFileExtensionsConstants.SymbolLongExtension,
                };
                //there are some document editors that we cannot open at the current stage
                if (!supportedFileExtensions.Contains(fileExtension))
                {
                    continue;
                }

                var file = await _solutionRepository.OpenDocumentAsync(slnNode);

                var result = await _fileCompiler.Compile(file);

                if (result != null)
                {
                    compileResults.Add(result);
                }
            }
            catch (Exception ex)
            {
                SendOutputMessage($"Error: {ex.Message}");
                SendErrorMessage(ex.Message, slnNode.Name, project.Name);
            }

        }

        //collect errors (we do it in a separate loop so that the prev loop be executed in Parallel)
        foreach (var file in compileResults)
        {
            foreach (var err in file.Errors)
            {
                SendOutputMessage($"Error: {err.Description}");
                SendErrorMessage(err);
            }
        }

        var hasErrors = compileResults.Any(f => f.Success == false);
        var resultMessage = hasErrors ? "with errors" : "successfully";

        SendOutputMessage($"Project {project.Name} compiled {resultMessage}");

        return !hasErrors;
    }

    public Task<bool> CompileProject(string filePath)
    {
        throw new NotImplementedException();
    }

    private void SendOutputMessage(string message)
    {
        StrongReferenceMessenger.Default.Send(message);
    }

    private void SendErrorMessage(IErrorMessage errorMessage)
    {
        StrongReferenceMessenger.Default.Send(errorMessage);
    }
    private void SendErrorMessage(string message, string fileName, string projectName)
    {
        var errorMessage = new ErrorMessage
        {
            Severity = MessageSeverity.Error,
            Description = message,
            File = fileName,
            Project = projectName
        };
        StrongReferenceMessenger.Default.Send(errorMessage);
    }
}
