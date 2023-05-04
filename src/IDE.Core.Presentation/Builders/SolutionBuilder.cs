using System;
using System.IO;
using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Messages;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Presentation.Solution;
using IDE.Core.Storage;
using IDE.Core.Utilities;

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

    //private List<string> outputFiles = new List<string>();
    private readonly ISolutionRepository _solutionRepository;
    private readonly IObjectFinder _objectFinder;
    private readonly ISchematicBuilder _schematicBuilder;
    private readonly IBoardBuilder _boardBuilder;

    public async Task<IList<string>> BuildSolution(string solutionFilePath)
    {
        var outputFiles = new List<string>();

        var solutionProjectFiles = _solutionRepository.GetProjectsFromSolution(solutionFilePath);

        foreach (var projectFile in solutionProjectFiles)
        {
            var projectOutputFiles = await BuildProject(projectFile);

            outputFiles.AddRange(projectOutputFiles);
        }

        var files = CreateSolutionOutput(Path.GetDirectoryName(solutionFilePath), outputFiles);

        return files;
    }


    public async Task<IList<string>> BuildProject(string projectFilePath)
    {
        var project = _solutionRepository.LoadProjectDocument(projectFilePath);

        switch (project.OutputType)
        {
            case ProjectOutputType.Library:
                return await BuildLibraryProject(projectFilePath, project);

            case ProjectOutputType.Board:
                return await BuildGerberProject(projectFilePath);
        }

        return new List<string>();
    }

    private IList<string> CreateSolutionOutput(string solutionFolder, IList<string> outputFiles)
    {
        //save output
        var savePath = Path.Combine(solutionFolder, "!Output");//folder
        Directory.CreateDirectory(savePath);
        var solutionOutputFiles = new List<string>();

        foreach (var fileOutput in outputFiles)
        {
            var fileName = Path.GetFileName(fileOutput);
            var solutionOutputFile = Path.Combine(savePath, fileName);
            File.Copy(fileOutput, solutionOutputFile, true);

            solutionOutputFiles.Add(solutionOutputFile);
        }

        return solutionOutputFiles;
    }

    private async Task<IList<string>> BuildLibraryProject(string projectFilePath, ProjectDocument projectDoc)
    {
        var outputFiles = new List<string>();

        var projectInfo = new ProjectInfo
        {
            ProjectPath = projectFilePath,
            Project = projectDoc
        };
        var projectFolder = Path.GetDirectoryName(projectFilePath);
        var projectName = Path.GetFileNameWithoutExtension(projectFilePath);

        OutputMessage($"Building project library {projectName}");

        var projectFiles = FileSystemHelper.GetFilesWithExtension(projectFolder, new[] { ".symbol", ".footprint", ".component", ".model" });

        var fileDocuments = new List<LibraryItem>();
        foreach (var filePath in projectFiles)
        {
            var fileDoc = await _solutionRepository.LoadLibraryItemAsync(filePath);

            if (fileDoc != null)
                fileDocuments.Add(fileDoc);
        }

        var symbols = fileDocuments.OfType<Symbol>().ToList();
        var footprints = fileDocuments.OfType<Footprint>().ToList();
        var components = fileDocuments.OfType<ComponentDocument>().ToList();
        var models = fileDocuments.OfType<ModelDocument>().ToList();

        //footprints will reference models from other libs (add them to our lib as libName = 'local')
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
                            modelSearch = _objectFinder.FindObject<ModelDocument>(projectInfo, model.ModelLibrary, model.ModelId);
                            if (modelSearch != null)
                            {
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
                            symbolSearch = _objectFinder.FindObject<Symbol>(projectInfo, gate.LibraryName, gate.symbolId);
                            if (symbolSearch != null)
                            {
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
                        fptSearch = _objectFinder.FindObject<Footprint>(projectInfo, cmp.Footprint.LibraryName, cmp.Footprint.footprintId);
                        if (fptSearch != null)
                        {
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
                                        modelSearch = _objectFinder.FindObject<ModelDocument>(projectInfo, model.ModelLibrary, model.ModelId);
                                        if (modelSearch != null)
                                        {
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

        var libDoc = new LibraryDocument { Name = projectName };
        if (projectDoc.Properties != null)
        {
            libDoc.Namespace = projectDoc.Properties.BuildOutputNamespace;
            libDoc.Version = projectDoc.Properties.Version;
        }

        libDoc.Symbols = symbols;
        libDoc.Footprints = footprints;
        libDoc.Components = components;
        libDoc.Models = models;

        //save output
        var savePath = Path.Combine(projectFolder, "!Output");//folder
        var outputFolder = savePath;
        Directory.CreateDirectory(savePath);
        savePath = Path.Combine(savePath, libDoc.Name + ".library");
        XmlHelper.Save(libDoc, savePath);

        OutputMessage($"Library built to: {savePath}");

        outputFiles.Add(savePath);

        return outputFiles;
    }

    private async Task<IList<string>> BuildGerberProject(string projectFilePath)
    {
        var outputFiles = new List<string>();

        var projectFolder = Path.GetDirectoryName(projectFilePath);
        var projectName = Path.GetFileNameWithoutExtension(projectFilePath);

        OutputMessage($"Building project {projectName}");

        //var projectFiles = FilterFiles(projectFolder, new[] { ".board", ".schematic"});

        //load board items
        var brdFiles = new List<IFileBaseViewModel>();
        var brdFilePaths = FileSystemHelper.GetFilesWithExtension(projectFolder, new[] { ".board" });
        foreach (var brdFilePath in brdFilePaths)
        {
            var file = await _solutionRepository.OpenDocumentAsync(brdFilePath);
            brdFiles.Add(file);
        }

        //load schematic files
        var schFiles = new List<IFileBaseViewModel>();
        var schFilePaths = FileSystemHelper.GetFilesWithExtension(projectFolder, new[] { ".schematic" });
        foreach (var schFilePath in schFilePaths)
        {
            var file = await _solutionRepository.OpenDocumentAsync(schFilePath);
            schFiles.Add(file);
        }

        var outputFolder = Path.Combine(projectFolder, "!Output");//folder
        Directory.CreateDirectory(outputFolder);

        foreach (IBoardDesigner board in brdFiles)
        {
            var buildResult = await _boardBuilder.Build(board, outputFolder);

            outputFiles.AddRange(buildResult.OutputFiles);
        }

        foreach (ISchematicDesigner sch in schFiles)
        {
            var buildResult = await _schematicBuilder.Build(sch, outputFolder);

            outputFiles.AddRange(buildResult.OutputFiles);
        }


        OutputMessage($"Project built to: {outputFolder}");

        return outputFiles;
    }



    private void OutputMessage(string message)
    {
        Messenger.Send(message);
    }


}
