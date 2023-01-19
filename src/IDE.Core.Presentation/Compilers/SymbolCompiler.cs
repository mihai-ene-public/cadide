using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Solution;

namespace IDE.Core.Presentation.Compilers;

public class SymbolCompiler : AbstractCompiler, ISymbolCompiler
{
    private readonly ISolutionRepository _solutionRepository;

    public SymbolCompiler(ISolutionRepository solutionRepository)
    {
        _solutionRepository = solutionRepository;
    }
    public async Task<CompilerResult> Compile(ISymbolDesignerViewModel symbol)
    {
        var projectPath = _solutionRepository.GetProjectFilePath(symbol.FilePath);
        var projectName = Path.GetFileNameWithoutExtension(projectPath);

        var canvasModel = symbol.CanvasModel;
        var errors = new List<IErrorMessage>();
        var isValid = true;

        await Task.CompletedTask;

        //multiple pins in the same location
        var gpins = canvasModel.Items.OfType<PinCanvasItem>().GroupBy(p => new { p.X, p.Y }).Where(g => g.Count() > 1);

        foreach (var g in gpins)
        {
            var msg = $"Error: There are {g.Count()} pins at the same location: ({g.Key.X}, {g.Key.Y})";
            errors.Add(BuildErrorMessage(msg, projectName, symbol));
        }

        isValid = gpins.Count() == 0;

        return new CompilerResult
        {
            Success = isValid,
            Errors = errors
        };
    }
}