using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Storage;

namespace IDE.Core.Presentation.Compilers;

public class BoardCompiler : AbstractCompiler, IBoardCompiler
{
    private readonly IObjectFinder _objectFinder;

    public BoardCompiler(IObjectFinder objectFinder)
    {
        _objectFinder = objectFinder;
    }

    public async Task<CompilerResult> Compile(IBoardDesigner board)
    {
        var isValid = true;
        var project = board.ProjectNode;
        var canvasModel = board.CanvasModel;
        var errors = new List<IErrorMessage>();

        //todo: check schematic if exists

        //todo: items on layers that don't exist

        //check components
        //todo: we could also check the defined component for its symbols, footprints, etc
        foreach (var part in canvasModel.GetFootprints())
        {
            try
            {
                if (part.PartName != null)
                {
                    //todo: changing the componentId and footprintId doesn't report missing component or footprint
                    //var cmpSearch = project.FindObject(TemplateType.Component, part.FootprintPrimitive.ComponentLibrary, part.FootprintPrimitive.ComponentId);
                    var cmpSearch = _objectFinder.FindObject<ComponentDocument>(project.Project, part.FootprintPrimitive.ComponentLibrary, part.FootprintPrimitive.ComponentId);
                    if (cmpSearch == null)
                        throw new Exception($"Part {part.PartName} was not found");

                    //var fptSearch = project.FindObject(TemplateType.Footprint, part.FootprintPrimitive.Library, part.FootprintPrimitive.FootprintId);
                    var fptSearch = _objectFinder.FindObject<Footprint>(project.Project, part.FootprintPrimitive.Library, part.FootprintPrimitive.FootprintId);
                    if (fptSearch == null)
                        throw new Exception($"Footprint for part '{part.PartName}' was not found");
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                var error = BuildErrorMessage(ex.Message, project.Name, board);
                errors.Add(error);
            }
        }

        //check connected nets
        var unroutedConnections = await board.GetUnroutedConnections();
        foreach (var c in unroutedConnections)
        {
            isValid = false;
            errors.Add(BuildErrorMessage($"Net {c.Net} is not completely routed", project.Name, board));
        }


        var checkRules = true;//todo: a setting

        if (checkRules)
        {
            var brdChecker = new BoardRulesCompiler();
            var res = await brdChecker.Compile(board);
            if (!res.Success)
            {
                isValid = false;
                errors.AddRange(res.Errors);
            }
        }

        return new CompilerResult()
        {
            Success = isValid,
            Errors = errors
        };
    }
}
