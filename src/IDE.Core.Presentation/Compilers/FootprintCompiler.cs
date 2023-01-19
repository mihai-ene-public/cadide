using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Presentation.Solution;
using IDE.Core.Storage;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Compilers;

public class FootprintCompiler : AbstractCompiler, IFootprintCompiler
{
    private readonly IObjectFinder<ModelDocument> _modelFinder;
    private readonly ISolutionRepository _solutionRepository;

    public FootprintCompiler(IObjectFinder<ModelDocument> modelFinder, ISolutionRepository solutionRepository)
    {
        _modelFinder = modelFinder;
        _solutionRepository = solutionRepository;
    }


    public async Task<CompilerResult> Compile(IFootprintDesigner footprint)
    {
        var hasErrors = false;
        var projectPath = _solutionRepository.GetProjectFilePath(footprint.FilePath);
        var project = _solutionRepository.LoadProjectDocument(projectPath);
        var errors = new List<IErrorMessage>();
        var projectInfo = new ProjectInfo
        {
            Project = project,
            ProjectPath = projectPath,
        };

        await Task.CompletedTask;

        //check reference of the model
        var models = footprint.GetModels();
        if (models != null)
        {
            foreach (var model in models)
            {
                try
                {
                    if (model.Name != null)
                    {
                        var modelSearch = _modelFinder.FindObject(projectInfo, model.Library, model.Id);
                        if (modelSearch == null)
                            throw new Exception($"Model {model.Name} was not found");
                    }
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    errors.Add(BuildErrorMessage(ex.Message, Path.GetFileNameWithoutExtension(projectPath), footprint));
                }
            }
        }

        return new CompilerResult
        {
            Success = !hasErrors,
            Errors = errors
        };
    }
}