using System.IO;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Presentation.Solution;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Storage;

namespace IDE.Core.Presentation.Compilers;

public class BoardCompiler : AbstractCompiler, IBoardCompiler
{
    private readonly IObjectFinder _objectFinder;
    private readonly ISolutionRepository _solutionRepository;


    public BoardCompiler(IObjectFinder objectFinder, ISolutionRepository solutionRepository)
    {
        _objectFinder = objectFinder;
        _solutionRepository = solutionRepository;
    }

    public async Task<CompilerResult> Compile(IBoardDesigner board)
    {
        var isValid = true;
        var projectPath = _solutionRepository.GetProjectFilePath(board.FilePath);
        var project = _solutionRepository.LoadProjectDocument(projectPath);
        var projectName = Path.GetFileNameWithoutExtension(projectPath);
        var projectInfo = new ProjectInfo
        {
            Project = project,
            ProjectPath = projectPath,
        };

        var canvasModel = board;
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
                    var cmpSearch = _objectFinder.FindObject<ComponentDocument>(projectInfo, part.FootprintPrimitive.ComponentLibrary, part.FootprintPrimitive.ComponentId);
                    if (cmpSearch == null)
                        throw new Exception($"Part {part.PartName} was not found");

                    var fptSearch = _objectFinder.FindObject<Footprint>(projectInfo, part.FootprintPrimitive.Library, part.FootprintPrimitive.FootprintId);
                    if (fptSearch == null)
                        throw new Exception($"Footprint for part '{part.PartName}' was not found");
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                var error = BuildErrorMessage(ex.Message, projectName, board);
                errors.Add(error);
            }
        }

        //check connected nets
        var unroutedConnections = await board.GetUnroutedConnections();
        foreach (var c in unroutedConnections)
        {
            isValid = false;
            errors.Add(BuildErrorMessage($"Net {c.Net} is not completely routed", projectName, board));
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
