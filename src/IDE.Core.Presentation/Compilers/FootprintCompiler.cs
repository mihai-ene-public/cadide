using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Storage;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Compilers;

public class FootprintCompiler : AbstractCompiler, IFootprintCompiler
{
    private readonly IObjectFinder<ModelDocument> _modelFinder;

    public FootprintCompiler(IObjectFinder<ModelDocument> modelFinder)
    {
        _modelFinder = modelFinder;
    }

    public async Task<CompilerResult> Compile(IFootprintDesigner footprint)
    {
        var hasErrors = false;
        var project = footprint.ProjectNode;
        var errors = new List<IErrorMessage>();

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
                        var modelSearch = _modelFinder.FindObject(project.Project, model.Library, model.Id); //project.FindObject(TemplateType.Model, model.Library, model.Id);
                        if (modelSearch == null)
                            throw new Exception($"Model {model.Name} was not found");
                    }
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    errors.Add(BuildErrorMessage(ex.Message, project.Name, footprint));
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