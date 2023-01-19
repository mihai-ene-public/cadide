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

public class ComponentCompiler : AbstractCompiler, IComponentCompiler
{
    private readonly IObjectFinder _objectFinder;
    private readonly ISolutionRepository _solutionRepository;

    public ComponentCompiler(IObjectFinder objectFinder, ISolutionRepository solutionRepository)
    {
        _objectFinder = objectFinder;
        _solutionRepository = solutionRepository;
    }

    public async Task<CompilerResult> Compile(IComponentDesigner component)
    {
        var slnNodeName = component.FileName;
        var projectPath = _solutionRepository.GetProjectFilePath(component.FilePath);
        var project = _solutionRepository.LoadProjectDocument(projectPath);
        var projectName = Path.GetFileNameWithoutExtension(projectPath);
        var hasErrors = false;
        var errors = new List<IErrorMessage>();
        var projectInfo = new ProjectInfo
        {
            Project = project,
            ProjectPath = projectPath,
        };

        await Task.CompletedTask;

        //prefix not specified
        if (string.IsNullOrEmpty(component.Prefix))
        {
            var msg = $"Component prefix not specified for {slnNodeName})";
            errors.Add(BuildErrorMessage(msg, projectName, component));
            hasErrors = true;
        }

        var gates = component.Gates;
        //at least one gate
        if (gates == null || gates.Count == 0)
        {
            var msg = $"Component has no gates specified for {slnNodeName})";
            errors.Add(BuildErrorMessage(msg, projectName, component));
            hasErrors = true;
        }

        //gate references
        foreach (var gate in gates)
        {
            try
            {
                if (gate.Name != null)
                {
                    //var symbolSearch = project.FindObject(TemplateType.Symbol, gate.Gate.LibraryName, gate.Gate.symbolId);
                    var symbolSearch = _objectFinder.FindObject<Symbol>(projectInfo, gate.Gate.LibraryName, gate.Gate.symbolId);
                    if (symbolSearch == null)
                        throw new Exception($"Symbol {gate.Symbol.Name} was not found");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errors.Add(BuildErrorMessage(ex.Message, projectName, component));
            }
        }

        //footprint references
        var footprint = component.Footprint;
        if (footprint != null)
        {
            try
            {
                if (footprint.Name != null)
                {
                    var fptSearch = _objectFinder.FindObject<Footprint>(projectInfo, footprint.Footprint.Library, footprint.Footprint.Id);
                    if (fptSearch == null)
                        throw new Exception($"Footprint {footprint.Name} was not found");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errors.Add(BuildErrorMessage(ex.Message, projectName, component));
            }
        }

        return new CompilerResult
        {
            Success = !hasErrors,
            Errors = errors
        };
    }
}