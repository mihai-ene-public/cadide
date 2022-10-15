using System;
using System.IO;
using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Documents;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Presentation.Solution;
using IDE.Core.Storage;
using IDE.Core.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace IDE.Core.Presentation.Builders;

public class SolutionBuilder : ISolutionBuilder
{
    public SolutionBuilder(
                        ISolutionRepository solutionRepository,
                        IObjectFinder objectFinder,
                        ISchematicBuilder schematicBuilder,
                        IBoardBuilder boardBuilder)
    {
        _solutionRepository = solutionRepository;
        _objectFinder = objectFinder;
        _schematicBuilder = schematicBuilder;
        _boardBuilder = boardBuilder;
    }

    private List<string> outputFiles = new List<string>();
    private readonly ISolutionRepository _solutionRepository;
    private readonly IObjectFinder _objectFinder;
    private readonly ISchematicBuilder _schematicBuilder;
    private readonly IBoardBuilder _boardBuilder;

    public async Task BuildSolution(string solutionFilePath)
    {
        outputFiles.Clear();

        var solutionProjects = _solutionRepository.GetProjectsFromSolution(solutionFilePath);

        //create build order
        var orderedProjects = CreateBuildOrder(solutionProjects);

        foreach (var project in orderedProjects)
        {
            await BuildProject(project);
        }

        CreateSolutionOutput(Path.GetDirectoryName(solutionFilePath));
    }

    public async Task BuildSolution(ISolutionRootNodeModel solution)
    {
        outputFiles.Clear();

        var solutionProjects = new List<ISolutionProjectNodeModel>();
        _solutionRepository.LoadSolutionProjects(solution, solutionProjects);

        //create build order
        var orderedProjects = CreateBuildOrder(solutionProjects);

        foreach (var project in orderedProjects)
        {
            await BuildProject(project);
        }

        CreateSolutionOutput(solution.GetItemFolderFullPath());
    }

    public async Task BuildProject(ISolutionProjectNodeModel project)
    {
        switch (project.Project.OutputType)
        {
            case ProjectOutputType.Library:
                await BuildLibraryProject(project);
                break;
            case ProjectOutputType.Board:
                await BuildGerberProject(project);
                break;
        }
    }

    public Task BuildProject(string projectFilePath)
    {
        throw new NotImplementedException();
    }

    private IList<ISolutionProjectNodeModel> CreateBuildOrder(IList<ISolutionProjectNodeModel> solutionProjects)
    {
        //first libraries, then gerbers
        var orderedProjects = solutionProjects.OrderBy(p => p.Project.OutputType).ToList();

        return orderedProjects;
    }


    private void CreateSolutionOutput(string solutionFolder)
    {
        //save output
        var savePath = Path.Combine(solutionFolder, "!Output");//folder
        Directory.CreateDirectory(savePath);

        foreach (var fileOutput in outputFiles)
        {
            var fileName = Path.GetFileName(fileOutput);
            File.Copy(fileOutput, Path.Combine(savePath, fileName), true);
        }
    }

    private async Task BuildLibraryProject(ISolutionProjectNodeModel project)
    {
        OutputMessage($"Building project library {project.Name}");

        //create a linear list;
        var libraryItemNodes = new List<ISolutionExplorerNodeModel>();
        CreateProjectItemsLinearList(project.Children.ToList(), libraryItemNodes);

        //load library items
        //var files = new List<IFileBaseViewModel>();
        //foreach (var slnNode in libraryItemNodes)
        //{
        //    var file = await _solutionRepository.OpenDocumentAsync(slnNode);
        //    files.Add(file);
        //}

        //var fileDocuments = files.Where(f => f.Document != null).Select(f => f.Document).ToList();
        var fileDocuments = new List<LibraryItem>();
        foreach (var slnNode in libraryItemNodes)
        {
            var filePath = slnNode.GetItemFullPath();
            var fileDoc = await _solutionRepository.LoadLibraryItemAsync(filePath);

            if (fileDoc != null)
                fileDocuments.Add(fileDoc);
        }

        var symbols = fileDocuments.OfType<Symbol>().ToList();
        var footprints = fileDocuments.OfType<Footprint>().ToList();
        var components = fileDocuments.OfType<ComponentDocument>().ToList();
        var models = fileDocuments.OfType<ModelDocument>().ToList();
        //var fonts = fileDocuments.OfType<FontDocument>().ToList();

        //footprints will reference models from other libs (add them to our lib as libName = 'local'
        #region Models in footprints

        foreach (var fp in footprints)
        {
            if (fp.Models != null)
            {
                foreach (var model in fp.Models)
                {
                    try
                    {
                        var modelSearch = models.FirstOrDefault(m => m.Id == model.ModelId);

                        if (modelSearch == null)
                        {
                            modelSearch = _objectFinder.FindObject<ModelDocument>(project.Project, model.ModelLibrary, model.ModelId);
                            if (modelSearch != null)
                            {
                                // modelSearch.Library = null;
                                models.Add(modelSearch);
                            }
                        }
                    }
                    catch //(Exception ex)
                    {
                    }
                }
            }
        }
        #endregion

        //component references symbols and footprints (and models)
        #region Component references

        foreach (var cmp in components)
        {
            //symbols
            if (cmp.Gates != null)
            {
                foreach (var gate in cmp.Gates)
                {
                    try
                    {
                        var symbolSearch = symbols.FirstOrDefault(s => s.Id == gate.symbolId);
                        if (symbolSearch == null)
                        {
                            symbolSearch = _objectFinder.FindObject<Symbol>(project.Project, gate.LibraryName, gate.symbolId);
                            if (symbolSearch != null)
                            {
                                //symbolSearch.Library = null;
                                symbols.Add(symbolSearch);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            //footprint references
            if (cmp.Footprint != null)
            {
                try
                {
                    var fptSearch = footprints.FirstOrDefault(f => f.Id == cmp.Footprint.footprintId);
                    if (fptSearch == null)
                    {
                        fptSearch = _objectFinder.FindObject<Footprint>(project.Project, cmp.Footprint.LibraryName, cmp.Footprint.footprintId);
                        if (fptSearch != null)
                        {
                            //fptSearch.Library = null;
                            footprints.Add(fptSearch);
                        }
                    }

                    //footprint models
                    if (fptSearch != null)
                    {
                        if (fptSearch.Models != null)
                        {
                            foreach (var model in fptSearch.Models)
                            {
                                try
                                {
                                    var modelSearch = models.FirstOrDefault(m => m.Id == model.ModelId);

                                    if (modelSearch == null)
                                    {
                                        modelSearch = _objectFinder.FindObject<ModelDocument>(project.Project, model.ModelLibrary, model.ModelId);
                                        if (modelSearch != null)
                                        {
                                            // modelSearch.Library = null;
                                            models.Add(modelSearch);
                                        }
                                    }
                                }
                                catch //(Exception ex)
                                {
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        #endregion

        var libDoc = new LibraryDocument { Name = project.Name };
        if (project.Project != null && project.Project.Properties != null)
        {
            libDoc.Namespace = project.Project.Properties.BuildOutputNamespace;
            libDoc.Version = project.Project.Properties.Version;
        }

        libDoc.Symbols = symbols;
        libDoc.Footprints = footprints;
        libDoc.Components = components;
        libDoc.Models = models;

        //save output
        var savePath = Path.Combine(project.GetItemFolderFullPath(), "!Output");//folder
        var outputFolder = savePath;
        Directory.CreateDirectory(savePath);
        savePath = Path.Combine(savePath, libDoc.Name + ".library");
        XmlHelper.Save(libDoc, savePath);

        ////save each font file in output doc
        //foreach (var fontDoc in fonts)
        //{
        //    var fontPath = Path.Combine(outputFolder, fontDoc.Name + ".font");
        //    XmlHelper.Save(fontDoc, fontPath);

        //    outputFiles.Add(fontPath);
        //}


        OutputMessage($"Library built to: {savePath}");

        outputFiles.Add(savePath);
    }

    private async Task BuildGerberProject(ISolutionProjectNodeModel project)
    {
        OutputMessage($"Building project {project.Name}");
        //todo: highlight errors with red and warnings with orange
        //Error: errors message /n
        //Warning: warning message /n

        //create a linear list;
        var libraryItemNodes = new List<ISolutionExplorerNodeModel>();
        CreateProjectItemsLinearList(project.Children.ToList(), libraryItemNodes);

        //load board items
        var brdFiles = new List<IFileBaseViewModel>();
        foreach (ISolutionExplorerNodeModel slnNode in libraryItemNodes.OfType<IProjectBoardNodeModel>())
        {
            var file = await _solutionRepository.OpenDocumentAsync(slnNode);
            brdFiles.Add(file);
        }

        //load schematic files
        var schFiles = new List<IFileBaseViewModel>();
        foreach (ISolutionExplorerNodeModel slnNode in libraryItemNodes.OfType<IProjectSchematicNodeModel>())
        {
            var file = await _solutionRepository.OpenDocumentAsync(slnNode);
            schFiles.Add(file);
        }

        //maybe do some processing: update references based on preferences
        //...

        //save into a virtual entity
        //...


        foreach (IBoardDesigner board in brdFiles)
        {
            var buildResult = await _boardBuilder.Build(board);

            outputFiles.AddRange(buildResult.OutputFiles);
        }

        foreach (ISchematicDesigner sch in schFiles)
        {
            var buildResult = await _schematicBuilder.Build(sch);

            outputFiles.AddRange(buildResult.OutputFiles);
        }

        var outputFolder = Path.Combine(project.GetItemFolderFullPath(), "!Output");//folder
        OutputMessage($"Project built to: {outputFolder}");
    }

    private void CreateProjectItemsLinearList(List<ISolutionExplorerNodeModel> children, List<ISolutionExplorerNodeModel> libraryItems)
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

    private void OutputMessage(string message)
    {
        StrongReferenceMessenger.Default.Send(message);
    }


}
