using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IDE.Core.Interfaces;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Compilers;

public class ComponentCompiler : AbstractCompiler, IComponentCompiler
{
    public async Task<CompilerResult> Compile(IComponentDesigner component)
    {
        var slnNodeName = component.FileName;
        var project = component.ProjectNode;
        var projectName = project.Name;
        var hasErrors = false;
        var errors = new List<IErrorMessage>();

        await Task.CompletedTask;

        //prefix not specified
        if (string.IsNullOrEmpty(component.Prefix))
        {
            var msg = $"Component prefix not specified for {slnNodeName})";
            errors.Add(BuildErrorMessage(msg, project.Name, component));
            hasErrors = true;
        }

        var gates = component.Gates;
        //at least one gate
        if (gates == null || gates.Count == 0)
        {
            var msg = $"Component has no gates specified for {slnNodeName})";
            errors.Add(BuildErrorMessage(msg, project.Name, component));
            hasErrors = true;
        }

        //gate references
        foreach (var gate in gates)
        {
            try
            {
                if (gate.Name != null)
                {
                    var symbolSearch = project.FindObject(TemplateType.Symbol, gate.Gate.LibraryName, gate.Gate.symbolId);
                    if (symbolSearch == null)
                        throw new Exception($"Symbol {gate.Symbol.Name} was not found");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errors.Add(BuildErrorMessage(ex.Message, project.Name, component));
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
                    var fptSearch = project.FindObject(TemplateType.Footprint, footprint.Footprint.Library, footprint.Footprint.Id);
                    if (fptSearch == null)
                        throw new Exception($"Footprint {footprint.Name} was not found");
                }
            }
            catch (Exception ex)
            {
                hasErrors = true;
                errors.Add(BuildErrorMessage(ex.Message, project.Name, component));
            }
        }

        return new CompilerResult
        {
            Success = !hasErrors,
            Errors = errors
        };
    }
}